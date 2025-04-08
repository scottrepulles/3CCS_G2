using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.Blazor.AmbientContext;
using Microsoft.Extensions.DependencyInjection;
using DevExpress.ExpressApp.Blazor.Services;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DHK.Module.Enumerations;
using DHK.Blazor.Module.BusinessObjects.Globals;
using DHK.Blazor.Module.Helpers.Globals;

namespace DHK.Blazor.Module.Hangfire;

public class HangfireJobDataBinder(IServiceScopeFactory serviceScopeFactory) : JobFilterAttribute, IClientFilter, IServerFilter, IElectStateFilter, IApplyStateFilter
{
    private bool _isApplicationInitialized;

    public IServiceScopeFactory ServiceScopeFactory { get; } = serviceScopeFactory;

    private static HangfireJobStateType ToHfJobState(string stateName)
    {
        if (stateName == AwaitingState.StateName)
        {
            return HangfireJobStateType.Awaiting;
        }
        else if (stateName == DeletedState.StateName)
        {
            return HangfireJobStateType.Deleted;
        }
        else if (stateName == EnqueuedState.StateName)
        {
            return HangfireJobStateType.Enqueued;
        }
        else if (stateName == FailedState.StateName)
        {
            return HangfireJobStateType.Failed;
        }
        else if (stateName == ProcessingState.StateName)
        {
            return HangfireJobStateType.Processing;
        }
        else if (stateName == ScheduledState.StateName)
        {
            return HangfireJobStateType.Scheduled;
        }
        else if (stateName == SucceededState.StateName)
        {
            return HangfireJobStateType.Succeeded;
        }
        else
        {
            throw new Exception("Unknown state");
        }
    }

    private bool PrepareApplication()
    {
        if (!_isApplicationInitialized) {
            using (IServiceScope scope = ServiceScopeFactory?.CreateScope())
            {
                IValueManagerStorageContext valueManagerContext = scope.ServiceProvider?.GetRequiredService<IValueManagerStorageContext>();
                valueManagerContext.RunWithStorage(() =>
                {
                    valueManagerContext.EnsureStorage();

                    IXafApplicationProvider applicationProvider = scope.ServiceProvider.GetRequiredService<IXafApplicationProvider>();
                    if (applicationProvider != null)
                    {
                        BlazorApplication application = applicationProvider.GetApplication();
                        _isApplicationInitialized = application != null;
                    }
                });
            }
        }

        return _isApplicationInitialized;
    }

    private bool UpdateJobData(ApplyStateContext context)
    {
        bool result = false;

        string jobId = context.BackgroundJob.Id;
        string oldState = context.OldStateName;
        string newState = context.NewState.Name;

        using (IServiceScope scope = ServiceScopeFactory?.CreateScope())
        {
            ServiceCredentialsHelper.RunWithServiceCredentials(scope, () =>
            {
                IValueManagerStorageContext valueManagerContext = scope.ServiceProvider?.GetRequiredService<IValueManagerStorageContext>();
                if (valueManagerContext != null)
                {
                    valueManagerContext.RunWithStorage(() =>
                    {
                        valueManagerContext.EnsureStorage();

                        var securedObjectSpaceFactory = scope.ServiceProvider?.GetRequiredService<IObjectSpaceFactory>();

                        INonSecuredObjectSpaceFactory nonSecuredObjectSpaceFactory = scope.ServiceProvider?.GetRequiredService<INonSecuredObjectSpaceFactory>();
                        using (IObjectSpace objectSpace = nonSecuredObjectSpaceFactory?.CreateNonSecuredObjectSpace<BaseHangfireJob>())
                        //var securedObjectSpaceFactory = scope.ServiceProvider?.GetRequiredService<IObjectSpaceFactory>();
                        //using (IObjectSpace objectSpace = securedObjectSpaceFactory?.CreateObjectSpace<BaseHfJob>())
                        {
                            if (objectSpace is not null)
                            {
                                try
                                {
                                    BaseHangfireJob hfJobData = objectSpace.FindObject<BaseHangfireJob>(CriteriaOperator.Parse(
                                        "BackgroundJobId == ?",
                                        jobId
                                    ));

                                    if (hfJobData != null)
                                    {
                                        hfJobData.State = ToHfJobState(newState);
                                    }
                                    else
                                    {
                                        HangfireJobLog hangfireJob = objectSpace.FindObject<HangfireJobLog>(CriteriaOperator.Parse(
                                            "BackgroundJobId == ?",
                                            jobId
                                        ));

                                        if (hangfireJob != null)
                                        {
                                            hangfireJob.State = ToHfJobState(newState);
                                        }
                                    }
                                    objectSpace.CommitChanges();
                                    result = true;
                                }
                                catch (Exception)
                                {
                                    _isApplicationInitialized = false;
                                    result = false;
                                }
                            }
                        }
                    });
                }
            });
        }

        return result;
    }

    public void OnCreating(CreatingContext filterContext)
    {

    }

    public void OnCreated(CreatedContext filterContext)
    {

    }

    public void OnPerforming(PerformingContext filterContext)
    {

    }

    public void OnPerformed(PerformedContext filterContext)
    {

    }

    public void OnStateElection(ElectStateContext context)
    {

    }

    public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
    {
        bool isApplied = false;
        do
        {
            if (PrepareApplication())
            {
                isApplied = UpdateJobData(context);
            }
        } while (!isApplied);
    }

    public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
    {
        bool isApplied = false;
        do
        {
            if (PrepareApplication())
            {
                isApplied = UpdateJobData(context);
            }
        } while (!isApplied);
    }
}
