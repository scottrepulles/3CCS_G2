using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DHK.Module.Constants;
using DKH.Module.Constants;
using System.ComponentModel;
using DHK.Module.Converters;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DHK.Module.Helper;

namespace DHK.Module.BusinessObjects;

[DefaultClassOptions]
[ModelDefault(ModelDefaultProperties.IS_CLONEABLE, ModelDefaultProperties.IS_TRUE)]
public class ImportMapping(Session session) : AuditedType(session)
{
    protected override void OnDeleting()
    {
        base.OnDeleting();
    }

    string entity;
    [Browsable(false)]
    [Size(FieldSizes.NOTES)]
    [XafDisplayName(DisplayNames.ENTITY)]
    public string Entity
    {
        get => entity;
        set
        {
            Type type = XafTypesInfo.Instance.FindTypeInfo(value)?.Type;

            if (entityDataType != type)
            {
                entityDataType = type;
            }

            SetPropertyValue(nameof(Entity), ref entity, value);
        }
    }

    Type entityDataType;
    [RuleRequiredField(DefaultContexts.Save)]
    [ValueConverter(typeof(TypeToStringConverter))]
    [TypeConverter(typeof(ImportMappingClassInfoTypeConverter))]
    [XafDisplayName(DisplayNames.BUSINESS_OBJECT)]
    [ImmediatePostData]
    [NonPersistent]
    public Type EntityDataType
    {
        get => entityDataType;
        set
        {
            SetPropertyValue(nameof(EntityDataType), ref entityDataType, value);
            Entity = value?.FullName;
        }
    }

    [Association($"{nameof(ImportMapping)}{nameof(ImportMappingProperty)}"), DevExpress.Xpo.Aggregated]
    public XPCollection<ImportMappingProperty> Properties
    {
        get
        {
            XPCollection<ImportMappingProperty> collection = GetCollection<ImportMappingProperty>(nameof(Properties));
            SortingHelper.Sort(collection, nameof(ImportMappingProperty.SortOrder), DevExpress.Xpo.DB.SortingDirection.Ascending);
            return collection;
        }
    }
}
