using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DHK.Module.Constants;
using DHK.Module.Enumerations;
using DHK.Module.Interfaces;
using DKH.Module.Constants;

namespace DHK.Module.BusinessObjects;

[DefaultClassOptions]
public class Enrollment(Session session) : AuditedEntity(session), IImported
{
    public override void AfterConstruction()
    {
        base.AfterConstruction();
    }

    Student student;
    decimal grade;
    EnrollmentStatusType status;
    Section section;

    [Association($"{nameof(Enrollment)}{nameof(Student)}")]
    public Student Student
    {
        get => student;
        set => SetPropertyValue(nameof(Student), ref student, value);
    }

    //[ModelDefault(ModelDefaultProperties.DISPLAY_FORMAT, Patterns.DECIMAL)]
    //[ModelDefault(ModelDefaultProperties.EDIT_MASK, ModelDefaultProperties.NUMBER)]
    //[RuleRange($"{ModelDefaultProperties.RULE_RANGE}{nameof(Enrollment)}{nameof(Grade)}", DefaultContexts.Save, 0, 5)]
    //public decimal Grade
    //{
    //    get => grade;
    //    set => SetPropertyValue(nameof(Grade), ref grade, value);
    //}

    public EnrollmentStatusType Status
    {
        get => status;
        set => SetPropertyValue(nameof(Status), ref status, value);
    }

    [Association($"{nameof(Enrollment)}{nameof(Section)}")]
    public Section Section
    {
        get => section;
        set => SetPropertyValue(nameof(Section), ref section, value);
    }
}