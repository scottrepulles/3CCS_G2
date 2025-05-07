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
            IObjectSpace objectSpace = Application.CreateObjectSpace<Student>();
            if (SecuritySystem.CurrentUser is Student currentUser)
            {
                bool hasStudentRole = currentUser.Roles.Any(r => r.Name == RoleNames.STUDENTS);
                if (hasStudentRole)
                {
                    //List<Enrollment> enrollments  = objectSpace.GetObjectsQuery<Enrollment>().Where(o => o.Status == DHK.Module.Enumerations.EnrollmentStatusType.ACTIVE &&
                    //o.Student.Oid == currentUser.Oid).ToList();
                    //List<Enrollment> enrollments_ = View.ObjectSpace.GetObjectsQuery<Enrollment>().ToList();
                    //List<Guid> courseIds = [.. enrollments.Select(o => o.Section.Course.Oid)];
                    //courseIds = [.. courseIds.GroupBy(x => x).Select(g => g.First())];
                    CriteriaOperator courseCriteria = CriteriaOperator.Parse($"{nameof(Document.Syllabus)}.{nameof(Syllabus.Course)}.{nameof(Course.Program)}.{nameof(DHK.Module.BusinessObjects.Program.Oid)} = ?", currentUser.Program?.Oid);

                    // Filter by visibility
                    CriteriaOperator visibilityCriteria = new BinaryOperator(nameof(Document.Visible), true);

                    // Filter by expiration: either null or in the future
                    CriteriaOperator expirationCriteria = new GroupOperator(GroupOperatorType.Or,
                        new NullOperator(nameof(Document.ExpirationDate)),
                        new BinaryOperator(nameof(Document.ExpirationDate), DateTime.Now, BinaryOperatorType.Greater)
                    );
                    CriteriaOperator finalCriteria = CriteriaOperator.And(
                        courseCriteria,
                        visibilityCriteria,
                        expirationCriteria
                    );
                    View.CollectionSource.Criteria["DocumentCriteria"] = finalCriteria;
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
