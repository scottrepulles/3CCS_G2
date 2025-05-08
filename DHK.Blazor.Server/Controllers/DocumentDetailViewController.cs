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
using DHK.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DHK.Blazor.Server.Controllers
{
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ViewController.
    public partial class DocumentDetailViewController : ObjectViewController<DetailView, DHK.Module.BusinessObjects.Document>
    {
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public DocumentDetailViewController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();

            if (SecuritySystem.CurrentUser is Student currentUser &&
                View.CurrentObject is Document document)
            {
                using (IObjectSpace objectSpace = Application.CreateObjectSpace<Tracker>())
                {
                    // Check if a Tracker already exists for this student and document
                    var trackerExist = objectSpace.GetObjectsQuery<Tracker>()
                        .Where(t => t.CreatedBy.Oid == currentUser.Oid && t.Document.Oid == document.Oid)
                        .OrderByDescending(t => t.CreatedOn)
                        .FirstOrDefault();

                    if (trackerExist == null)
                    {
                        Tracker tracker = objectSpace.CreateObject<Tracker>();
                        tracker.Document = objectSpace.GetObject(document); // use local object space
                        tracker.CreatedBy = objectSpace.GetObject(currentUser);
                        tracker.ViewedBy = currentUser.FormattedFullName;
                        tracker.CreatedOn = DateTime.Now;

                        objectSpace.CommitChanges();
                    }
                }
            }
        }

        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
