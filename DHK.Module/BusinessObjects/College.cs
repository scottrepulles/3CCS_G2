using DevExpress.DataAccess.DataFederation;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DHK.Module.Constants;
using DHK.Module.Interfaces;
namespace DHK.Module.BusinessObjects;

[DefaultClassOptions]
public class College(Session session) : AuditedEntity(session), IImported
{
    public override void AfterConstruction()
    {
        base.AfterConstruction();
    }

    string name;
    string description;
    string code;


    [Size(SizeAttribute.DefaultStringMappingFieldSize)]
    [RuleRequiredField(DefaultContexts.Save)]
    public string Name
    {
        get => name;
        set => SetPropertyValue(nameof(Name), ref name, value);
    }

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