using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Microsoft.AspNetCore.Components;
using DHK.Blazor.Module.ValidationRules.Globals;
using DHK.Module.BusinessObjects;
using DHK.Module.Constants;
using DHK.Module.Converters;
using DHK.Module.Enumerations;
using DHK.Module.Interfaces;
using System.ComponentModel;

namespace DHK.Blazor.Module.BusinessObjects.Globals;

[DefaultClassOptions]
[XafDisplayName(DisplayNames.IMPORT)]
public abstract class BaseHangfireJob(Session session) : AuditedEntity(session), IHangfireJobData
{
    HangfireJobStateType state;
    string backgroundJobId;

    [Size(FieldSizes.NOTES)]
    [VisibleInDetailView(false)]
    [Browsable(false)]
    public string BackgroundJobId
    {
        get => backgroundJobId;
        set => SetPropertyValue(nameof(BackgroundJobId), ref backgroundJobId, value);
    }

    [ValueConverter(typeof(GenericEnumConverter<HangfireJobStateType>))]
    [VisibleInDetailView(false)]
    [VisibleInListView(false)]
    [Browsable(false)]
    public HangfireJobStateType State
    {
        get => state;
        set => SetPropertyValue(nameof(State), ref state, value);
    }

    [Browsable(false)]
    public HangfireJobStateType Status
    {
        get => State;
    }

    [VisibleInListView(false)]
    [Browsable(false)]
    public string JobDetails
    {
        get
        {
            if (Session.ServiceProvider.GetService(typeof(NavigationManager)) is NavigationManager navigationManager && !string.IsNullOrEmpty(BackgroundJobId))
            {
                return string.Format(BaseHangfireJobCriteria.JobDetailsFormat, navigationManager.BaseUri, BackgroundJobId);
            }
            return string.Empty;
        }
    }
}
