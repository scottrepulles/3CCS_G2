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
                    var enrollments = objectSpace.GetObjectsQuery<Enrollment>()
                        .Where(o => o.Status == DHK.Module.Enumerations.EnrollmentStatusType.ACTIVE &&
                                    o.Student.Oid == currentUser.Oid)
                        .ToList();

                    var courseIds = enrollments
                        .Where(o => o.Section.HideSyllabus)
                        .Select(o => o.Section.Course.Oid)
                        .Distinct()
                        .ToList();

                    var criteriaList = new List<CriteriaOperator>();

                    // Always apply program filter
                    criteriaList.Add(CriteriaOperator.Parse(
                        $"{nameof(Document.Syllabus)}.{nameof(Syllabus.Course)}.{nameof(Course.Program)}.{nameof(DHK.Module.BusinessObjects.Program.Oid)} = ?",
                        currentUser.Program?.Oid));

                    // Always apply visibility filter
                    criteriaList.Add(new BinaryOperator(nameof(Document.Visible), true));

                    // Always apply expiration filter
                    criteriaList.Add(new GroupOperator(GroupOperatorType.Or,
                        new NullOperator(nameof(Document.ExpirationDate)),
                        new BinaryOperator(nameof(Document.ExpirationDate), DateTime.Now, BinaryOperatorType.Greater)));

                    // Optionally apply course ID filter if we have valid course IDs
                    if (courseIds.Any())
                    {
                        criteriaList.Add(new UnaryOperator(UnaryOperatorType.Not, new InOperator(
                            $"{nameof(Document.Syllabus)}.{nameof(Syllabus.Course)}.{nameof(Course.Oid)}", courseIds)));
                    }

                    // Combine all criteria
                    CriteriaOperator finalCriteria = new GroupOperator(GroupOperatorType.And, criteriaList);
                    View.CollectionSource.Criteria["DocumentCriteria"] = finalCriteria;
                }
            }
            if (SecuritySystem.CurrentUser is Teacher currentTeacher)
            {
                bool hasTeacherRole = currentTeacher.Roles.Any(r => r.Name == RoleNames.TEACHERS);
                if (hasTeacherRole)
                {
                    var objectCriteria = CriteriaOperator.Parse($"{nameof(Syllabus.CreatedBy)}.{nameof(Syllabus.CreatedBy.Oid)} = ?", currentTeacher.Oid);
                    View.CollectionSource.Criteria["DocumentTeacherCriteria"] = objectCriteria;
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
