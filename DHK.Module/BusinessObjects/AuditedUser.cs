using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo;
using Microsoft.Extensions.DependencyInjection;
using DKH.Module.Constants;
using DHK.Module.Constants;

namespace DHK.Module.BusinessObjects
{
    [NonPersistent]
    public abstract class AuditedUser(Session session) : ApplicationUser(session)
    {
        protected override void OnSaving()
        {
            base.OnSaving();

            if (Session.IsNewObject(this))
            {
                SetPropertyValueWithSecurityBypass(nameof(CreatedBy), GetCurrentUser());
                SetPropertyValueWithSecurityBypass(nameof(CreatedOn), DateTime.Now);
            }
            else
            {
                SetPropertyValueWithSecurityBypass(nameof(UpdatedBy), GetCurrentUser());
                SetPropertyValueWithSecurityBypass(nameof(UpdatedOn), DateTime.Now);
            }
        }
        DateTime updatedOn;
        ApplicationUser updatedBy;
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
            set
            {
                SetPropertyValue(nameof(CreatedBy), ref createdBy, value);
            }
        }

        [NonCloneable]
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

        [NonCloneable]
        [ModelDefault(nameof(IModelCommonMemberViewItem.AllowEdit), ModelDefaultProperties.IS_FALSE)]
        [ModelDefault(ModelDefaultProperties.NULL_TEXT, ModelDefaultProperties.SYSTEM)]
        [VisibleInDashboards(false)]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [VisibleInReports(false)]
        [Indexed]
        public ApplicationUser UpdatedBy
        {
            get => updatedBy;
            set => SetPropertyValue(nameof(UpdatedBy), ref updatedBy, value);
        }

        [NonCloneable]
        [ModelDefault(nameof(IModelCommonMemberViewItem.AllowEdit), ModelDefaultProperties.IS_FALSE)]
        [ModelDefault(nameof(IModelCommonMemberViewItem.DisplayFormat), Patterns.GENERAL_SHORT_DATE_TIME_FORMAT_UTC_POSTFIX)]
        [VisibleInDashboards(false)]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [VisibleInReports(false)]
        [ValueConverter(typeof(UtcDateTimeConverter))]
        [Indexed]
        public DateTime UpdatedOn
        {
            get => updatedOn;
            set => SetPropertyValue(nameof(UpdatedOn), ref updatedOn, value);
        }

        ApplicationUser GetCurrentUser()
        {
            return Session.GetObjectByKey<ApplicationUser>(
                Session.ServiceProvider.GetRequiredService<ISecurityStrategyBase>().UserId);
        }

    }
}
