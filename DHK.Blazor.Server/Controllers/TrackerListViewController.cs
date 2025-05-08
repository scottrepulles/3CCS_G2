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
    public partial class TrackerListViewController : ObjectViewController<ListView, Tracker>
    {
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public TrackerListViewController()
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
            CriteriaOperator objectCriteria = null;
            IObjectSpace objectSpace = Application.CreateObjectSpace<Teacher>();
            if (SecuritySystem.CurrentUser is Teacher currentTeacher)
            {
                bool hasTeacherRole = currentTeacher.Roles.Any(r => r.Name == RoleNames.TEACHERS);
                if (hasTeacherRole)
                {
                    objectCriteria = CriteriaOperator.Parse($"{nameof(Tracker.Document)}.{nameof(Syllabus.CreatedBy)}.{nameof(Syllabus.CreatedBy.Oid)} = ?", currentTeacher.Oid);
                    View.CollectionSource.Criteria["TrackerCriteria"] = objectCriteria;
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
