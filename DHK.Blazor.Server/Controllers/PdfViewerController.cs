using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DHK.Module.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DHK.Blazor.Server.Controllers
{
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ViewController.
    public partial class PdfViewerController : ObjectViewController<ListView, PDFDocument>
    {
        public PdfViewerController()
        {
            var viewPdfAction = new SimpleAction(this, "OpenPdfViewer", PredefinedCategory.View)
            {
                Caption = "Preview PDF",
                ImageName = "Open"
            };
            viewPdfAction.Execute += ViewPdfAction_Execute;
        }

        private void ViewPdfAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var obj = e.CurrentObject as PDFDocument;
            var detailView = Application.CreateDetailView(View.ObjectSpace, obj);
            detailView.ViewEditMode = ViewEditMode.View;
            e.ShowViewParameters.CreatedView = detailView;
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
        }
    }

}
