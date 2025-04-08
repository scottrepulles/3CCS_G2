using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Microsoft.AspNetCore.Components;
using DHK.Module.BusinessObjects;
using DHK.Module.Constants;
using DHK.Module.Converters;
using DHK.Module.Enumerations;
using DHK.Module.Interfaces;

namespace DHK.Blazor.Module.BusinessObjects.Globals;

[NavigationItem(DisplayNames.AUTOMATION)]
[XafDisplayName(DisplayNames.HANGFIRE_JOB)]
public class HangfireJobLog(Session session) : AuditedEntity(session), IHangfireJobData
{

    HangfireJobStateType state;
    string backgroundJobId;
    string backgroundJobName;

    [VisibleInDetailView(false)]
    public string BackgroundJobId
    {
        get => backgroundJobId;
        set => SetPropertyValue(nameof(BackgroundJobId), ref backgroundJobId, value);
    }

    public string BackgroundJobName
    {
        get => backgroundJobName;
        set => SetPropertyValue(nameof(BackgroundJobName), ref backgroundJobName, value);
    }

    [ValueConverter(typeof(GenericEnumConverter<HangfireJobStateType>))]
    [VisibleInDetailView(false)]
    [VisibleInListView(false)]
    public HangfireJobStateType State
    {
        get => state;
        set => SetPropertyValue(nameof(State), ref state, value);
    }

    public HangfireJobStateType Status
    {
        get => State;
    }

    [VisibleInListView(false)]
    public string JobDetails
    {
        get
        {
            NavigationManager navigationManager = Session.ServiceProvider.GetService(typeof(NavigationManager)) as NavigationManager;
            if (navigationManager != null && !string.IsNullOrEmpty(BackgroundJobId))
            {
                return string.Format(CustomMessages.JOB_DETAILS_PATH, navigationManager.BaseUri, BackgroundJobId);
            }
            return "";
        }
    }


    [Association($"{nameof(HangfireJobLogMessage)}{nameof(HangfireJobLog)}"), DevExpress.Xpo.Aggregated]
    public XPCollection<HangfireJobLogMessage> JobLogs
    {
        get
        {
            return GetCollection<HangfireJobLogMessage>(nameof(JobLogs));
        }
    }

}
