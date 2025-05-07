using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DHK.Module.BusinessObjects;
using DHK.Module.Constants;

namespace DHK.Blazor.Server.Controllers
{
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ViewController.
    public partial class CourseListViewController : ObjectViewController<ListView, Course>
    {
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public CourseListViewController()
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
            IObjectSpace objectSpace = Application.CreateObjectSpace<Student>();
            if (SecuritySystem.CurrentUser is Teacher currentTeacher)
            {
                bool hasTeacherRole = currentTeacher.Roles.Any(r => r.Name == RoleNames.TEACHERS);
                if (hasTeacherRole)
                {
                    List<Section> enrollments  = objectSpace.GetObjectsQuery<Section>().Where(o => o.Teacher.Oid == currentTeacher.Oid).ToList();
                    List<Guid> courseIds = [.. enrollments.Select(o => o.Course.Oid)];
                    courseIds = [.. courseIds.GroupBy(x => x).Select(g => g.First())];
                    CriteriaOperator courseCriteria = new InOperator($"{nameof(Course.Oid)}", courseIds);
                    View.CollectionSource.Criteria["CourseCriteria"] = courseCriteria;
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
