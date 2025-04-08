using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.Spreadsheet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using DHK.Blazor.Module.Helpers.Globals;
using DHK.Module.Constants;
using System.Drawing;
using DHK.Module.BusinessObjects;
using DHK.Module.Interfaces;
using DevExpress.ClipboardSource.SpreadsheetML;

namespace DHK.Blazor.Module.Controllers.Imports;

public partial class ImportMappingViewController : ViewController
{
    private readonly SimpleAction createTemplateAction;
    private const string DHK_FOLDER = "";
    private const string IMPORT_FOLDER = DisplayNames.IMPORT;
    private const string TEMPLATE_FOLDER = DisplayNames.TEMPLATE;
    private readonly PopupWindowShowAction importAction;

    public ImportMappingViewController()
    {
        InitializeComponent();

        TargetObjectType = typeof(ImportMapping);

        createTemplateAction = new SimpleAction(this, ActionIdentifier.CREATE_IMPORT_TEMPLATE, string.Empty) 
        { 
            TargetViewType = ViewType.DetailView,
            Caption = DisplayNames.TEMPLATE,
            ImageName = ActionImageNames.BO_UNKNOWN
        };
       // createTemplateAction.Execute += CreateTemplateAction_Execute;

        importAction = new PopupWindowShowAction(this, ActionIdentifier.IMPORT_ENTITY, PredefinedCategory.View)
        {
            TargetViewType = ViewType.DetailView,
            SelectionDependencyType = SelectionDependencyType.Independent,
            Caption = DisplayNames.IMPORT,
            ImageName = ActionImageNames.BO_UNKNOWN,
            TargetObjectsCriteria = string.Empty
        };
        importAction.Execute += ImportAction_Execute;
        importAction.CustomizePopupWindowParams += ImportAction_CustomizePopupWindowParams;

        // Audit Event
        PopupWindowShowAction auditEventAction = new(this, $"{nameof(AuditedEvent)}{nameof(ImportMapping)}", PredefinedCategory.Edit)
        {
            Caption = DisplayNames.LOGS,
            ImageName = ActionImageNames.BO_AUDIT_CHANGE_HISTORY,
            TargetViewType = ViewType.DetailView,
            TargetObjectType = typeof(ImportMapping)
        };
        auditEventAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(this.AuditEventAction_CustomizePopupWindowParams);
    }

    private void ImportAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
    {
        IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(IHangfireJobData));

        Type objectType = ((ImportMapping)View.CurrentObject).EntityDataType;
        FileImportHelper.ShowPopupDetailView(Application, objectSpace, e, objectType, null);
    }

    private void ImportAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
    {

    }

    private void AuditEventAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs args)
    {
        ImportMapping importMapping = (ImportMapping)View.CurrentObject;
        FileImportHelper.ShowPopupAuditLogsDetail(Application, args, importMapping.EntityDataType);
    }

    //private void CreateTemplateAction_Execute(object sender, SimpleActionExecuteEventArgs e)
    //{
    //    IConfiguration configuration = Application.ServiceProvider.GetRequiredService<IConfiguration>();

    //    IObjectSpace objectSpace = View.ObjectSpace;
    //    ImportMapping importMapping = (ImportMapping)View.CurrentObject;
    //    string fileName = $"{importMapping.EntityDataType.Name}{DisplayNames.TEMPLATE}.xlsx";
    //    string workSheetName = $"{importMapping.EntityDataType.Name}{DisplayNames.TEMPLATE}";

    //    // Create a new workbook.

    //    Workbook workbook = new(); // No using
    //    DevExpress.Spreadsheet.Worksheet worksheet = workbook.Worksheets[0];
    //    worksheet.Name = workSheetName;
    //    workbook.Unit = DevExpress.Office.DocumentUnit.Point;

    //    workbook.BeginUpdate();

    //    try
    //    {
    //        IList<ImportMappingProperty> importMappingProperties = [.. importMapping.Properties.OrderBy(p => p.SortOrder)];
    //        int ctr = 0;

    //        foreach (ImportMappingProperty importMappingProperty in importMappingProperties)
    //        {
    //            bool alreadyExists = worksheet.Rows["1"]
    //                .Any(cell => cell.Value?.ToString() == importMappingProperty.MapTo);

    //            if (!alreadyExists)
    //            {
    //                if (importMappingProperty.Required)
    //                {
    //                    worksheet.Rows["1"][ctr].FillColor = Color.Yellow;
    //                }

    //                worksheet.Rows["1"][ctr].Value = importMappingProperty.MapTo;
    //                worksheet.Rows["2"][ctr].Value = importMappingProperty.SampleValue;
    //                ctr++;
    //            }
    //        }

    //        worksheet.Rows["1"].Font.Bold = true;
    //        CellRange tableRange = worksheet.GetDataRange();
    //        tableRange.ColumnWidth = 100;
    //    }
    //    finally
    //    {
    //        workbook.EndUpdate();
    //    }

    //    // Save document to byte array
    //    byte[] array = workbook.SaveDocument(DocumentFormat.OpenXml);

    //    // Define your local path
    //    string localDirectory = Path.Combine(AppContext.BaseDirectory, "ImportTemplates");
    //    Directory.CreateDirectory(localDirectory); // Ensure directory exists

    //    string fullFilePath = Path.Combine(localDirectory, fileName);

    //    // Save to disk
    //    File.WriteAllBytes(fullFilePath, array);

    //    // Generate a relative or virtual path for opening in the browser
    //    string virtualPath = $"/ImportTemplates/{fileName}";

    //    // Use JSInterop to open the file
    //    IJSRuntime JSRuntime = Application.ServiceProvider.GetRequiredService<IJSRuntime>();
    //    _ = JSRuntime.InvokeAsync<object>(CustomMessages.OPEN, CancellationToken.None, virtualPath, CustomMessages.BLANK);
    //}

}
