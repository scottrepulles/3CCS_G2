using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DHK.Module.Enumerations;

namespace DHK.Module.BusinessObjects;

[DefaultClassOptions]
public class Enrollment(Session session) : AuditedEntity(session)
{
    public override void AfterConstruction()
    {
        base.AfterConstruction();
    }

    Course course;
    Student student;
    AcademicYear academicYear;
    SemesterType semester;
    int grade;
    EnrollmentStatusType status;


    [Association($"{nameof(Enrollment)}{nameof(Course)}")]
    public Course Course
    {
        get => course;
        set => SetPropertyValue(nameof(Course), ref course, value);
    }

    [Association($"{nameof(Enrollment)}{nameof(Student)}")]
    public Student Student
    {
        get => student;
        set => SetPropertyValue(nameof(Student), ref student, value);
    }

    public AcademicYear AcademicYear
    {
        get => academicYear;
        set => SetPropertyValue(nameof(AcademicYear), ref academicYear, value);
    }

    public SemesterType Semester
    {
        get => semester;
        set => SetPropertyValue(nameof(Semester), ref semester, value);
    }

    public int Grade
    {
        get => grade;
        set => SetPropertyValue(nameof(Grade), ref grade, value);
    }

    public EnrollmentStatusType Status
    {
        get => status;
        set => SetPropertyValue(nameof(Status), ref status, value);
    }
}