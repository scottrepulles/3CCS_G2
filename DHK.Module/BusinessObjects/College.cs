using DevExpress.DataAccess.DataFederation;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
namespace DHK.Module.BusinessObjects;

[DefaultClassOptions]
public class College(Session session) : AuditedEntity(session)
{
    public override void AfterConstruction()
    {
        base.AfterConstruction();
    }

    string name;
    string description;

    [Size(SizeAttribute.DefaultStringMappingFieldSize)]
    [RuleRequiredField(DefaultContexts.Save)]
    public string Name
    {
        get => name;
        set => SetPropertyValue(nameof(Name), ref name, value);
    }

    [Size(SizeAttribute.DefaultStringMappingFieldSize)]
    [RuleRequiredField(DefaultContexts.Save)]
    public string Description
    {
        get => description;
        set => SetPropertyValue(nameof(Description), ref description, value);
    }

    [Association($"{nameof(College)}{nameof(Program)}")]
    [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
    public XPCollection<Program> Programs
    {
        get
        {
            return GetCollection<Program>(nameof(Programs));
        }
    }
}