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
                objectCriteria = CriteriaOperator.Parse($"{nameof(Course)}.{nameof(Course.Program)}.{nameof(DHK.Module.BusinessObjects.Program.Oid)} = ?", currentUser.Program?.Oid);
                View.CollectionSource.Criteria["SyllabusCriteria"] = objectCriteria;
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
