using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.MultiTenancy;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DHK.Module.Constants;
using DKH.Module.Constants;
using System.ComponentModel;

namespace DHK.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DefaultProperty(nameof(Name))]
    public class BaseTenant(Session session) : BaseObject(session), ITenant, ITenantWithConnectionString
    {
        string name;
        string connectionString;
        string apiUrl;

        Guid ITenant.Id => Oid;

        [RuleRequiredField($"{nameof(RuleRequiredField)}{nameof(BaseTenant)}{nameof(Name)}", DefaultContexts.Save)]
        [RuleUniqueValue($"{nameof(RuleUniqueValue)}{nameof(BaseTenant)}{nameof(Name)}", DefaultContexts.Save)]
        [Indexed(Unique = true)]
        [Size(FieldSizes.NOTES)]
        [RuleRegularExpression(null, DefaultContexts.Save, Patterns.ALPHA_NUMERIC_STARTING_WITH_LETTER, $"Only alphanumeric characters are allowed for {nameof(Name)}")]
        public string Name
        {
            get => name;
            set => SetPropertyValue(nameof(Name), ref name, value);
        }

        [RuleRequiredField($"{nameof(RuleRequiredField)}{nameof(BaseTenant)}{nameof(ConnectionString)}", DefaultContexts.Save)]
        [RuleUniqueValue($"{nameof(RuleUniqueValue)}{nameof(BaseTenant)}{nameof(ConnectionString)}", DefaultContexts.Save)]
        [ObjectValidatorIgnoreIssue([typeof(ObjectValidatorLargeNonDelayedMember)])]
        [Size(FieldSizes.BUFFER)]
        [VisibleInListView(false)]
        public string ConnectionString
        {
            get => connectionString;
            set => SetPropertyValue(nameof(ConnectionString), ref connectionString, value);
        }

        [VisibleInListView(false)]
        [Size(FieldSizes.NOTES)]
        public string ApiUrl
        {
            get => apiUrl;
            set => SetPropertyValue(nameof(ApiUrl), ref apiUrl, value);
        }
    }
}