using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DHK.Module.Constants;
using DKH.Module.Constants;
using Microsoft.Extensions.DependencyInjection;

namespace DHK.Module.BusinessObjects;

[NonPersistent]
public abstract class CreationAuditedEntity(Session session) : BaseObject(session)
{
    protected override void OnSaving()
    {
        base.OnSaving();

        if (Session.IsNewObject(this))
        {
            SetPropertyValueWithSecurityBypass(nameof(CreatedBy), GetCurrentUser());
            SetPropertyValueWithSecurityBypass(nameof(CreatedOn), DateTime.Now);
        }
    }

    ApplicationUser GetCurrentUser()
    {
        return Session.GetObjectByKey<ApplicationUser>(
            Session.ServiceProvider.GetRequiredService<ISecurityStrategyBase>().UserId);
    }

    DateTime createdOn;
    ApplicationUser createdBy;

    [NonCloneable]
    [ModelDefault(nameof(IModelCommonMemberViewItem.AllowEdit), ModelDefaultProperties.IS_FALSE)]
    [ModelDefault(ModelDefaultProperties.NULL_TEXT, ModelDefaultProperties.SYSTEM)]
    [VisibleInDashboards(false)]
    [VisibleInDetailView(false)]
    [VisibleInListView(false)]
    [VisibleInLookupListView(false)]
    [VisibleInReports(false)]
    [Indexed]
    public ApplicationUser CreatedBy
    {
        get => createdBy;
        set => SetPropertyValue(nameof(CreatedBy), ref createdBy, value);
    }

    [ModelDefault(nameof(IModelCommonMemberViewItem.AllowEdit), ModelDefaultProperties.IS_FALSE)]
    [ModelDefault(nameof(IModelCommonMemberViewItem.DisplayFormat), Patterns.GENERAL_SHORT_DATE_TIME_FORMAT_UTC_POSTFIX)]
    [VisibleInDashboards(false)]
    [VisibleInDetailView(false)]
    [VisibleInListView(false)]
    [VisibleInLookupListView(false)]
    [VisibleInReports(false)]
    [ValueConverter(typeof(UtcDateTimeConverter))]
    [Indexed]
    public DateTime CreatedOn
    {
        get => createdOn;
        set => SetPropertyValue(nameof(CreatedOn), ref createdOn, value);
    }
}