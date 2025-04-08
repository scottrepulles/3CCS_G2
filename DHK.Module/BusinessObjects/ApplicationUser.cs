using System.ComponentModel;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DHK.Module.ValidationRules;
using DKH.Module.Constants;

namespace DHK.Module.BusinessObjects;

[MapInheritance(MapInheritanceType.ParentTable)]
[DefaultProperty(nameof(UserName))]
public class ApplicationUser : PermissionPolicyUser, ISecurityUserWithLoginInfo, ISecurityUserLockout {
    private int accessFailedCount;
    private DateTime lockoutEnd;
    bool emaiConfirmed;
    MediaDataObject photo;
    long? phoneNumber;
    string email;

    public ApplicationUser(Session session) : base(session) { }

    [Browsable(false)]
    public int AccessFailedCount {
        get { return accessFailedCount; }
        set { SetPropertyValue(nameof(AccessFailedCount), ref accessFailedCount, value); }
    }

    [Browsable(false)]
    public DateTime LockoutEnd {
        get { return lockoutEnd; }
        set { SetPropertyValue(nameof(LockoutEnd), ref lockoutEnd, value); }
    }

    [Browsable(false)]
    [Aggregated, Association("User-LoginInfo")]
    public XPCollection<ApplicationUserLoginInfo> LoginInfo {
        get { return GetCollection<ApplicationUserLoginInfo>(nameof(LoginInfo)); }
    }

    IEnumerable<ISecurityUserLoginInfo> IOAuthSecurityUser.UserLogins => LoginInfo.OfType<ISecurityUserLoginInfo>();

    ISecurityUserLoginInfo ISecurityUserWithLoginInfo.CreateUserLoginInfo(string loginProviderName, string providerUserKey) {
        ApplicationUserLoginInfo result = new ApplicationUserLoginInfo(Session);
        result.LoginProviderName = loginProviderName;
        result.ProviderUserKey = providerUserKey;
        result.User = this;
        return result;
    }

    [ModelDefault(ModelDefaultProperties.EDIT_MASK, Patterns.PHONE_EDIT_MASK)]
    [ModelDefault(ModelDefaultProperties.DISPLAY_FORMAT, Patterns.PHONE_DISPLAY_FORMAT)]
    [VisibleInLookupListView(true)]
    [RuleRequiredField(DefaultContexts.Save)]
    public long? PhoneNumber
    {
        get => phoneNumber;
        set => SetPropertyValue(nameof(PhoneNumber), ref phoneNumber, value);
    }

    [RuleRegularExpression(EmailCriteria.Context, EmailCriteria.Pattern, CustomMessageTemplate = EmailCriteria.Message)]
    [RuleRequiredField(DefaultContexts.Save)]
    [Size(SizeAttribute.DefaultStringMappingFieldSize)]
    [ImmediatePostData]
    public string Email
    {
        get => email;
        set => SetPropertyValue(nameof(Email), ref email, value);
    }

    public bool EmailConfirmed
    {
        get => emaiConfirmed;
        set => SetPropertyValue(nameof(EmailConfirmed), ref emaiConfirmed, value);
    }
}
