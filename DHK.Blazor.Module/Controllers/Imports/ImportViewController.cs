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
using DKH.Module.Interfaces;
using DHK.Module.Enumerations;
using DHK.Module.Helper;
using DHK.Module.Interfaces;
using DHK.Module.BusinessObjects;


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
            //CreateTemplateAction_Execute();
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

    //private void CreateTemplateAction_Execute()
    //{
    //    IConfiguration configuration = Application.ServiceProvider.GetRequiredService<IConfiguration>();
    //    string storageKey = configuration["Azure:Storage:AccountKey"];
    //    string storageUri = configuration["Azure:Storage:AccountUri"];
    //    string storageName = configuration["Azure:Storage:AccountName"];
    //    StorageSharedKeyCredential sharedKeyCredential = new StorageSharedKeyCredential(storageName, storageKey);

    //    BlobServiceClient blobServiceClient = new BlobServiceClient
    //        (new Uri(storageUri), sharedKeyCredential);

    //    //Todo: dynamic container name per company for multilingual
    //    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("import-templates");

    //    IObjectSpace objectSpace = View.ObjectSpace;
    //    Type objectType = this.View.ObjectTypeInfo.Type;
    //    string fileName = $"{objectType.Name}Template.xlsx";
    //    string workSheetName = $"{objectType.Name}Template";

    //    //Create a new workbook.
    //    using (Workbook workbook = new())
    //    {

    //        Worksheet worksheet = new()
    //        {
    //            Name = workSheetName
    //        };
    //        workbook.Unit = DevExpress.Office.DocumentUnit.Point;

    //        workbook.BeginUpdate();

    //        try
    //        {
    //            IList<ImportMappingProperty> importMappingProperties = objectSpace.GetObjects<ImportMappingProperty>(CriteriaOperator.Parse(
    //                $"{nameof(ImportMapping)}.Entity='{objectType}'")).OrderBy(o => o.SortOrder).ToList();

    //            int ctr = 0;
    //            foreach (ImportMappingProperty importMappingProperty in importMappingProperties)
    //            {
    //                bool alreadyExists = false;
    //                foreach (var cell in worksheet.Rows["1"])
    //                {
    //                    if (cell.Value != null && cell.Value.ToString() == importMappingProperty.MapTo)
    //                    {
    //                        alreadyExists = true;
    //                        break;
    //                    }
    //                }
    //                if (!alreadyExists)
    //                {
    //                    worksheet.Rows["1"][ctr].Value = $"{importMappingProperty.MapTo}";
    //                    worksheet.Rows["2"][ctr].Value = $"{importMappingProperty.SampleValue}";
    //                    ctr += 1;
    //                }
    //            }

    //            worksheet.Rows["1"].Font.Bold = true;
    //            CellRange tableRange = worksheet.GetDataRange();
    //            tableRange.ColumnWidth = 100;
    //        }
    //        finally
    //        {
    //            workbook.EndUpdate();
    //        }
    //        byte[] array = workbook.SaveDocument(DocumentFormat.OpenXml);
    //        Stream stream = new MemoryStream(array);
    //        BlobClient blobClient = containerClient.GetBlobClient(fileName);
    //        if (blobClient != null)
    //        {
    //            blobClient.Upload(stream, true);
    //        }
    //        else
    //        {
    //            containerClient.UploadBlob(fileName, stream);
    //            blobClient = containerClient.GetBlobClient(fileName);
    //        }

    //        BlobSasBuilder sasBuilder = new BlobSasBuilder()
    //        {
    //            BlobContainerName = containerClient.Name,
    //            BlobName = blobClient.Name,
    //            Resource = "b",
    //            StartsOn = DateTimeOffset.UtcNow,
    //            ExpiresOn = DateTimeOffset.UtcNow.AddHours(1),
    //        };

    //        sasBuilder.SetPermissions(BlobSasPermissions.Read);
    //        string sasToken = sasBuilder.ToSasQueryParameters(sharedKeyCredential).ToString();
    //        Uri sasUri = new UriBuilder(blobClient.Uri)
    //        {
    //            Query = sasToken
    //        }.Uri;

    //        string url = sasUri.AbsoluteUri;
    //        IJSRuntime JSRuntime = Application.ServiceProvider.GetRequiredService<IJSRuntime>();
    //        _ = JSRuntime.InvokeAsync<object>("open", CancellationToken.None, url, "_blank");
    //    }
    //}

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
