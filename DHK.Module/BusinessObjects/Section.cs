using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DHK.Module.Enumerations;
using DHK.Module.Interfaces;

namespace DHK.Module.BusinessObjects;

[DefaultClassOptions]
 public class Section(Session session) : AuditedEntity(session), IImported
{
    public override void AfterConstruction()
    {
        base.AfterConstruction();
    }

    Course course;
    Teacher teacher;
    AcademicYear academicYear;
    SemesterType semester;
    string name;
    string schedule;
    string room;
    string code;

    [XafDisplayName("Section")]
    public string Name
    {
        get => name;
        set => SetPropertyValue(nameof(Name), ref name, value);
    }

    public string Schedule
    {
        get => schedule;
        set => SetPropertyValue(nameof(Schedule), ref schedule, value);
    }

    public string Room
    {
        get => room;
        set => SetPropertyValue(nameof(Room), ref room, value);
    }

    [Association($"{nameof(Course)}{nameof(Section)}")]
    public Course Course
    {
        get => course;
        set => SetPropertyValue(nameof(Course), ref course, value);
    }

    [Association($"{nameof(Teacher)}{nameof(Section)}")]
    public Teacher Teacher
    {
        get => teacher;
        set => SetPropertyValue(nameof(Teacher), ref teacher, value);
    }

    public AcademicYear AcademicYear
    {
        get => academicYear;
        set => SetPropertyValue(nameof(AcademicYear), ref academicYear, value);
    }

    [Association($"{nameof(Enrollment)}{nameof(Section)}")]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    public XPCollection<Enrollment> Enrollments
    {
        get
        {
            return GetCollection<Enrollment>(nameof(Enrollments));
        }
    }

    public string Code
    {
        get => code;
        set => SetPropertyValue(nameof(Code), ref code, value);
    }

}