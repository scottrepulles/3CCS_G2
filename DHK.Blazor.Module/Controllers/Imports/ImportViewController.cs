using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Spreadsheet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using DHK.Blazor.Module.Helpers.Globals;
using DHK.Module.Constants;
using DHK.Module.Enumerations;
using DHK.Module.Helper;
using DHK.Module.Interfaces;
using DHK.Module.BusinessObjects;
using System.Drawing;
using DHK.Blazor.Module.Interfaces;


namespace DHK.Blazor.Module.Controllers.Imports;

public partial class ImportViewController : ViewController
{
    public SingleChoiceAction ImportAction;
    public ImportViewController()
    {
        InitializeComponent();

        TargetObjectType = typeof(IImported);
        TargetViewType = ViewType.ListView;

        ImportAction = new SingleChoiceAction(this, $"{DisplayNames.IMPORT_ACTIONS}", DevExpress.Persistent.Base.PredefinedCategory.FullTextSearch)
        {
            ImageName = "Action_CreateDashboard",
            ItemType = SingleChoiceActionItemType.ItemIsOperation,
            Caption = "Import",
            ToolTip = ""
        };
        ChoiceActionHelper.FillItemWithEnumValues(ImportAction, typeof(ActionImportType));
        ImportAction.Execute += ImportAction_Execute;
    }
    private void ImportAction_Execute(object sender, SingleChoiceActionExecuteEventArgs args)
    {
        if (args.SelectedChoiceActionItem.Data.ToString() == ActionImportType.Import.ToString())
        {
            ImportAction_Execute(args);
        }
        else if (args.SelectedChoiceActionItem.Data.ToString() == ActionImportType.ImportLogs.ToString())
        {
            ImportLogsAction_Execute(args);
        }
        else if (args.SelectedChoiceActionItem.Data.ToString() == ActionImportType.CreateTemplate.ToString())
        {
            CreateTemplateAction_Execute();
        }
    }
    private void ImportAction_Execute(SingleChoiceActionExecuteEventArgs args)
    {
        args.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
        args.ShowViewParameters.Context = TemplateContext.PopupWindow;
        ImportAction_CustomizePopupWindowParams(args);
        args.ShowViewParameters.CreateAllControllers = true;
        //dialog.Accepting += CreateViewVariantAction_Execute;
    }
    private void ImportLogsAction_Execute(SingleChoiceActionExecuteEventArgs args)
    {
        args.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
        args.ShowViewParameters.Context = TemplateContext.PopupWindow;
        AuditEventAction_CustomizePopupWindowParams(args);
        DialogController dialog = (Application.CreateController<DialogController>());
        args.ShowViewParameters.Controllers.Add(dialog);
        args.ShowViewParameters.CreateAllControllers = true;
    }
    private void ImportAction_CustomizePopupWindowParams(SingleChoiceActionExecuteEventArgs e)
    {
        IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(IHangfireJobData));

        Type objectType = this.View.ObjectTypeInfo.Type;
        object masterObject = null;
        if(View is ListView listView)
        {
            if (listView.CollectionSource is PropertyCollectionSource collectionSource)
            {
                if (collectionSource.MasterObject is not null)
                {
                    masterObject = collectionSource.MasterObject;
                }
            }
        }
        FileImportHelper.ShowPopupListlView(Application, objectSpace, e, objectType, masterObject);
    }

    private void AuditEventAction_CustomizePopupWindowParams(SingleChoiceActionExecuteEventArgs args)
    {
        Type objectType = this.View.ObjectTypeInfo.Type;
        FileImportHelper.ShowPopupAuditLogs(Application, args, objectType);
    }

    private void CreateTemplateAction_Execute()
    {
        IObjectSpace objectSpace = View.ObjectSpace;
        Type objectType = this.View.ObjectTypeInfo.Type;

        string fileName = $"{objectType.Name}Template.xlsx";
        string workSheetName = $"{objectType.Name}Template";

        // Create a new workbook.
        using Workbook workbook = new();

        Worksheet worksheet = workbook.Worksheets[0];
        worksheet.Name = workSheetName;
        workbook.Unit = DevExpress.Office.DocumentUnit.Point;

        workbook.BeginUpdate();

        try
        {
            var importMappingProperties = objectSpace.GetObjectsQuery<ImportMappingProperty>()
                .Where(o => o.ImportMapping.Entity == objectType.FullName).OrderBy(o => o.SortOrder).ToList();

            int ctr = 0;
            foreach (ImportMappingProperty importMappingProperty in importMappingProperties)
            {
                bool alreadyExists = worksheet.Rows["1"].Any(cell => cell.Value?.ToString() == importMappingProperty.MapTo);
                if (!alreadyExists)
                {
                    worksheet.Rows["1"][ctr].Value = importMappingProperty.MapTo;
                    worksheet.Rows["2"][ctr].Value = importMappingProperty.SampleValue;
                    ctr++;
                }
            }   
            worksheet.Rows["1"].Font.Bold = true;
            worksheet.GetDataRange().ColumnWidth = 100;
        }
        finally
        {
            workbook.EndUpdate();
        }

        // Save to local storage
        byte[] fileBytes = workbook.SaveDocument(DocumentFormat.OpenXml);
        ILocalFileService localFileService = Application.ServiceProvider.GetRequiredService<ILocalFileService>();
        string url = localFileService.SaveExcelTemplate(fileBytes, fileName);

        // Open the file in a new tab
        IJSRuntime JSRuntime = Application.ServiceProvider.GetRequiredService<IJSRuntime>();
        _ = JSRuntime.InvokeAsync<object>("open", CancellationToken.None, url, "_blank");
    }



    public void ImportAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
    {
    }

    protected override void OnActivated()
    {
        base.OnActivated();
    }
    protected override void OnViewControlsCreated()
    {
        base.OnViewControlsCreated();
    }
    protected override void OnDeactivated()
    {
        base.OnDeactivated();
    }
}
