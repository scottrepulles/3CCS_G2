using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DHK.Module.BusinessObjects;
using DHK.Module.Constants;
using System.Reflection.Metadata;
using Document = DHK.Module.BusinessObjects.Document;

namespace DHK.Blazor.Server.Controllers
{
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ViewController.
    public partial class DocumentListViewController : ObjectViewController<ListView, Document>
    {
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public DocumentListViewController()
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
            // Access and customize the target View control.
            CriteriaOperator objectCriteria = null;
            IObjectSpace objectSpace = Application.CreateObjectSpace<Student>();
            if (SecuritySystem.CurrentUser is Student currentUser)
            {
                bool hasStudentRole = currentUser.Roles.Any(r => r.Name == RoleNames.STUDENTS);
                if (hasStudentRole)
                {
                    objectCriteria = CriteriaOperator.Parse($"{nameof(Document.Syllabus)}.{nameof(Syllabus.Course)}.{nameof(Course.Program)}.{nameof(DHK.Module.BusinessObjects.Program.Oid)} = ?", currentUser.Program?.Oid);
                    View.CollectionSource.Criteria["DocumentCriteria"] = objectCriteria;
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
