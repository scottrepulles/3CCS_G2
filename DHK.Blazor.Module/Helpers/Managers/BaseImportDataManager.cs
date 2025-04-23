using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Spreadsheet;
using DevExpress.Spreadsheet.Export;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Hangfire.Console;
using Hangfire.Console.Progress;
using Hangfire.Server;
using DHK.Blazor.Module.BusinessObjects.Globals;
using DHK.Module.Constants;
using DHK.Module.Interfaces.Globals;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using DataColumn = System.Data.DataColumn;
using DataTable = System.Data.DataTable;
using DHK.Module.Enumerations;
using DHK.Module.BusinessObjects;
using DHK.Module.Helper;
using DevExpress.ClipboardSource.SpreadsheetML;

namespace DHK.Blazor.Module.Helpers.Managers;

public abstract class BaseImportDataManager<T, D>(
    IServiceProvider serviceProvider,
    IObjectSpace objectSpace,
    PerformContext performContext,
    string backgroundJobId,
    string parentObjectOid,
    string parentObjectType,
    string mappingId
    ) : IImportDataManager
    where T : BaseObject
    where D : FileImportJob
{
    protected readonly IServiceProvider ServiceProvider = serviceProvider;
    protected readonly IObjectSpace ObjectSpace = objectSpace;
    protected readonly PerformContext PerformContext = performContext;
    protected readonly string BackgroundJobId = backgroundJobId;
    protected readonly string ParentObjectOid = parentObjectOid;
    protected readonly string ParentObjectType = parentObjectType;
    protected readonly string MappingId = mappingId;

    protected D HangfireJobData { get; set; }

    public event AfterAssignedValueEventHandler AfterAssignedValue;
    public delegate void AfterAssignedValueEventHandler(object sender, XPBaseObject entity);

    public event AfterObjectCreatedEventHandler AfterObjectCreated;
    public delegate void AfterObjectCreatedEventHandler(object sender, XPBaseObject entity);

    protected abstract T GetMatchFromDb(IObjectSpace objectSpace, DataRow entityRow);
    protected virtual T CreateNewRecord(IObjectSpace objectSpace, DataRow entityRow) => objectSpace.CreateObject<T>();
    protected abstract T MatchSubProperty(IObjectSpace objectSpace, DataRow entityRow, T entity, MapperHelper mapper);

    protected virtual void SetProperty(
        IObjectSpace objectSpace,
        DataColumn entityTemplateColumn,
        DataRow entityRow,
        MapperHelper mapper,
        XPBaseObject entity
    )
    {
        if (entity != null)
        {
            string columnName = entityTemplateColumn.ColumnName;
            object entityColumnValue = entityRow[columnName];
            string propertyName = Regex.Replace(columnName, @"\s+", string.Empty); //remove spaces for property name
            XPMemberInfo targetMemberInfo = entity.ClassInfo.FindMember(propertyName);
            if (targetMemberInfo != null && entityColumnValue != null)
            {
                object clonedValue = mapper.GetPropertyValue(targetMemberInfo, entityColumnValue, objectSpace);
                if (clonedValue != null)
                {
                    targetMemberInfo.SetValue(entity, clonedValue);
                    AfterAssignedValue?.Invoke(this, entity);
                }
            }
        }
    }

    public virtual void ImportData()
    {
        PerformContext.WriteLine(CustomMessages.STARTING_IMPORT);
        DateTime importStart = DateTime.UtcNow;
        HangfireJobData = ObjectSpace.GetObjectsQuery<D>()
            .FirstOrDefault(job => job.BackgroundJobId == BackgroundJobId);
        FileData sourceImportFile = HangfireJobData.File;
        ImportDuplicateRecordType importDuplicateRecordType = HangfireJobData.ImportDuplicateRecordType;

        if (sourceImportFile == null) return;

        DevExpress.Spreadsheet.Workbook workbook = new DevExpress.Spreadsheet.Workbook();

        // Load a workbook from the file.
        if (sourceImportFile.FileName.ToLowerInvariant().EndsWith(".csv"))
        {
            workbook.LoadDocument(sourceImportFile.Content, DocumentFormat.Csv);
        }
        else
        {
            workbook.LoadDocument(sourceImportFile.Content, DocumentFormat.Xlsx);
        }

        Session session = ((XPObjectSpace)ObjectSpace).Session;
        MapperHelper mapper = new(session);

        // Access a collection of rows.
        RowCollection rows = workbook.Worksheets[0].Rows;
        DevExpress.Spreadsheet.Worksheet worksheet = workbook.Worksheets[0];
        CellRange cellRange = rows[0].Worksheet.GetUsedRange();

        DataTable entityDataTable = worksheet.CreateDataTable(cellRange, true);
        foreach (DataColumn leadTemplateColumn in entityDataTable.Columns)
        {
            leadTemplateColumn.DataType = Type.GetType("System.String");
        }

        //map headers and default value
        List<ImportMappingProperty> importMappingProperty = ObjectSpace.FindObject<ImportMapping>(
            CriteriaOperator.Parse($"{nameof(ImportMapping.Oid)}='{MappingId}'")).Properties.ToList();
        DataTableExporter exporter = worksheet.CreateDataTableExporter(cellRange, entityDataTable, true);

        // Handle value conversion errors.
        exporter.CellValueConversionError += Exporter_CellValueConversionError;
        exporter.Export();
        List<string> missingColumn = mapper.ValidateRequiredProperty(entityDataTable, importMappingProperty);
        if (missingColumn.Count > 0)
        {
            PerformContext.WriteLine($"{string.Join(", ", missingColumn)} {CustomMessages.MISSING_COLUMN_MESSAGE}");
            return;
        }

        if (cellRange.ColumnCount != entityDataTable.Columns.Count)
        {
            int rowCount = cellRange.RowCount;
            int newColumnCount = entityDataTable.Columns.Count;
            cellRange = cellRange.Resize(rowCount, newColumnCount);
        }

        IList<XPBaseObject> result = new List<XPBaseObject>();
        int invalidRecordCount = 0;
        XPBaseObject selectedObject = null;
        XPBaseObject entity = null;
        IProgressBar progressBar = PerformContext.WriteProgressBar();
        foreach (DataRow entityRow in entityDataTable.Rows.WithProgress(progressBar))
        {
            if (entityRow.ItemArray.All(item => item == null || string.IsNullOrWhiteSpace(item.ToString())))
            {
                // Row is entirely empty, handle accordingly
                continue;
            }

            try
            {
                T recordMatch = null;
                if (recordMatch != null)
                {
                    if (importDuplicateRecordType == ImportDuplicateRecordType.Skip) continue;
                    else entity = recordMatch;
                }
                else
                {
                    entity = CreateNewRecord(ObjectSpace, entityRow);
                    AfterObjectCreated?.Invoke(this, entity);
                }

                DataColumnCollection entityDataTableColumns = entityDataTable.Columns;
                foreach (DataColumn entityTemplateColumn in entityDataTableColumns)
                {
                    SetProperty(ObjectSpace, entityTemplateColumn, entityRow, mapper, entity);
                }
                entity = MatchSubProperty(ObjectSpace, entityRow, (T)entity, mapper);

                if (entity != null)
                {
                    XPMemberInfo targetMemberInfo = entity.ClassInfo.FindMember(nameof(AuditedEntity.CreatedBy));
                    if (targetMemberInfo != null)
                    {
                        ApplicationUser createdBy = ObjectSpace.GetObject(HangfireJobData.CreatedBy);
                        targetMemberInfo.SetValue(entity, createdBy);
                    }
                    bool isValid = false;
                    IRuleSet ruleSet = Validator.GetService(ServiceProvider);
                    foreach (object obj in ObjectSpace.ModifiedObjects)
                    {
                        RuleSetValidationResult validationResult = ruleSet.ValidateTarget(ObjectSpace, obj, new ContextIdentifiers(DefaultContexts.Save.ToString()));
                        isValid = validationResult.State != ValidationState.Invalid;
                        if (!isValid)
                        {
                            invalidRecordCount++;

                            if (invalidRecordCount == 1)
                            {
                                PerformContext.WriteLine(CustomMessages.DETAILS);
                                PerformContext.WriteLine(Environment.NewLine);
                            }
                            IEnumerable<RuleSetValidationResultItem> invalidStateResults = validationResult.Results.Where(r => r.State == ValidationState.Invalid);
                            foreach (RuleSetValidationResultItem validationResultItem in invalidStateResults)
                            {
                                PerformContext.WriteLine(CustomMessages.IMPORT_INVALID_MESSAGE, obj.GetType().FullName, validationResultItem.DisplayObjectName, validationResultItem.ErrorMessage);
                            }
                            IList modifiedObjects = ObjectSpace.ModifiedObjects;

                            // Roll back changes for each modified object
                            foreach (object modifiedObject in modifiedObjects)
                            {
                                if (modifiedObject.GetType() != typeof(Address))
                                {
                                    ObjectSpace.RemoveFromModifiedObjects(modifiedObject);
                                }
                            }
                            break;
                        }
                    }

                    if (isValid)
                    {
                        selectedObject ??= entity;
                        result.Add(entity);
                    }
                }
                ObjectSpace.CommitChanges();
                if (ObjectSpace is XPObjectSpace xpObjectSpace)
                {
                    xpObjectSpace.Session.CommitTransaction();
                }
            }
            catch (Exception ex)
            {
                IList modifiedObjects = ObjectSpace.ModifiedObjects;
                foreach (object modifiedObject in modifiedObjects)
                {
                    if (modifiedObject.GetType() != typeof(Address))
                    {
                        ObjectSpace.RemoveFromModifiedObjects(modifiedObject);
                    }
                }

                PerformContext.WriteLine(CustomMessages.IMPORT_ERROR_MESSAGE, entityDataTable.Rows.IndexOf(entityRow));
                PerformContext.WriteLine(ex.Message);
                PerformContext.WriteLine(ex.StackTrace);
            }

        }

        string message = string.Empty;

        if (invalidRecordCount == 0) message = string.Format("Finished! {0} data record(s) has/have been imported.", result.Count);
        else message = string.Format("Finished! {0} data record(s) has/have been imported. {1} data record(s) unable to import.", result.Count, invalidRecordCount);

        ObjectSpace.CommitChanges();

        DateTime importEnd = DateTime.UtcNow;
        TimeSpan importTime = importEnd - importStart;
        PerformContext.WriteLine("Import took: {0:dd\\.hh\\:mm\\:ss}", importTime);

        //ShowImportMessage(message, invalidRecordCount);
    }

    private void Exporter_CellValueConversionError(object sender, CellValueConversionErrorEventArgs e)
    {
        var xxx = "Error in cell " + e.Cell.GetReferenceA1();
        e.DataTableValue = null;
        e.Action = DataTableExporterAction.Continue;
    }

    //protected void ShowImportMessage(string message, int invalidRecordCount)
    //{
    //    if (BeforeShowImportMessage != null) BeforeShowImportMessage(this, message);
    //    MessageManager messageManager = new MessageManager(ServiceProvider);
    //    if (invalidRecordCount > 0) messageManager.ShowMessage("Success", message, InformationType.Warning);
    //    else messageManager.ShowMessage("Success", message);
    //}
}
