using DevExpress.DataAccess.DataFederation;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DHK.Module.Constants;
using DHK.Module.Interfaces;
using System.ComponentModel;

namespace DHK.Module.BusinessObjects;

[DefaultClassOptions]
[DefaultProperty(nameof(Name))]
public class Program(Session session) : AuditedEntity(session), IImported, IAuditEvent
{
    string code;
    string name;
    string description;
    College college;
    int duration;

    [NonCloneable]
    [RuleUniqueValue]
    [Indexed(Unique = true)]
    [Size(50)]
    [ModelDefault(ModelDefaultProperties.PROPERTY_EDITOR_ALLOW_EDIT, ModelDefaultProperties.IS_FALSE)]
    public string Code
    {
        get => code;
        set => SetPropertyValue(nameof(Code), ref code, value);
    }

    [NonCloneable]
    [Size(100)]
    [RuleRequiredField(DefaultContexts.Save)]
    public string Name
    {
        get => name;
        set => SetPropertyValue(nameof(Name), ref name, value);
    }

    [NonCloneable]
    [Size(100)]
    public string Description
    {
        get => description;
        set => SetPropertyValue(nameof(Description), ref description, value);
    }

    [CollectionOperationSet(AllowAdd = false, AllowRemove = false)]
    [Browsable(false)]
    public XPCollection<AuditDataItemPersistent> AuditEvents
    {
        get
        {
            return GetCollection<AuditDataItemPersistent>(nameof(AuditEvents));
        }
    }

    [Association($"{nameof(Program)}{nameof(Course)}")]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    public XPCollection<Course> Courses
    {
        get
        {
            return GetCollection<Course>(nameof(Courses));
        }
    }

    public int Duration
    {
        get => duration;
        set => SetPropertyValue(nameof(Duration), ref duration, value);
    }

    [Association($"{nameof(College)}{nameof(Program)}")]
    public College College
    {
        get => college;
        set => SetPropertyValue(nameof(College), ref college, value);
    }
}
