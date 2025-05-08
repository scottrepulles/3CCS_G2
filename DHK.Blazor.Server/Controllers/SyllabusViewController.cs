using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DHK.Module.BusinessObjects;
using DHK.Module.Constants;

namespace DHK.Blazor.Server.Controllers;

public partial class SyllabusViewController : ObjectViewController<ListView, Syllabus>
{
    public SyllabusViewController()
    {
        InitializeComponent();
    }
    protected override void OnActivated()
    {
        base.OnActivated();
    }
    protected override void OnViewControlsCreated()
    {
        base.OnViewControlsCreated();
        CriteriaOperator objectCriteria = null;
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
                criteriaList.Add(CriteriaOperator.Parse($"{nameof(Course)}.{nameof(Course.Program)}.{nameof(DHK.Module.BusinessObjects.Program.Oid)} = ?", currentUser.Program?.Oid));

                if (courseIds.Any())
                {
                    criteriaList.Add(new UnaryOperator(UnaryOperatorType.Not, new InOperator(
                        $"{nameof(Syllabus.Course)}.{nameof(Course.Oid)}", courseIds)));
                }

                CriteriaOperator finalCriteria = new GroupOperator(GroupOperatorType.And, criteriaList);
                View.CollectionSource.Criteria["SyllabusCriteria"] = finalCriteria;;
            }
        }
        if (SecuritySystem.CurrentUser is Teacher currentTeacher)
        {
            bool hasTeacherRole = currentTeacher.Roles.Any(r => r.Name == RoleNames.TEACHERS);
            if (hasTeacherRole)
            {
                objectCriteria = CriteriaOperator.Parse($"{nameof(Syllabus.CreatedBy)}.{nameof(Syllabus.CreatedBy.Oid)} = ?", currentTeacher.Oid);
                View.CollectionSource.Criteria["SyllabusCriteria"] = objectCriteria;
            }
        }
    }
    protected override void OnDeactivated()
    {
        base.OnDeactivated();
    }
}
