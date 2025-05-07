using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DHK.Module.BusinessObjects;
using DHK.Module.Constants;

namespace DHK.Blazor.Server.Controllers
{
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ViewController.
    public partial class SectionListViewController : ObjectViewController<ListView, DHK.Module.BusinessObjects.Section>
    {
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public SectionListViewController()
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
            // Access and customize the target View control
            CriteriaOperator objectCriteria = null;
            IObjectSpace objectSpace = Application.CreateObjectSpace<Student>();
            if (SecuritySystem.CurrentUser is Teacher currentTeacher)
            {
                bool hasTeacherRole = currentTeacher.Roles.Any(r => r.Name == RoleNames.TEACHERS);
                if (hasTeacherRole)
                {
                    objectCriteria = CriteriaOperator.Parse($"{nameof(Section.Teacher)}.{nameof(Section.Teacher.Oid)} = ?", currentTeacher.Oid);
                    View.CollectionSource.Criteria["SectionCriteria"] = objectCriteria;
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
