using DevExpress.ExpressApp;
using DHK.Blazor.Module.BusinessObjects.Globals;
using DHK.Blazor.Module.BusinessObjects.Schedules;
using DHK.Blazor.Module.Helpers.Globals;
using DHK.Module.Interfaces;

namespace DHK.Blazor.Module.Controllers.HangfireJobs;

public class HangfireJobDataDetailViewController : ObjectViewController<DetailView, IHangfireJobData>
{
    protected override void OnActivated()
    {
        base.OnActivated();
        ObjectSpace.Committing += ObjectSpace_Committing;
    }

    protected override void OnDeactivated()
    {
        base.OnDeactivated();
        ObjectSpace.Committing -= ObjectSpace_Committing;
    }

    public virtual void ObjectSpace_Committing(object sender, System.ComponentModel.CancelEventArgs e)
    {

        IHangfireJobData baseHfJob = View.CurrentObject as IHangfireJobData;
        if (baseHfJob is not null)
        {
            if (baseHfJob.BackgroundJobId == null && !((BaseHangfireJob)baseHfJob).IsDeleted)
            {
                if (baseHfJob is RecurringHangfireJob recurringHfJob)
                {
                    JobProcessHelper.ExecuteRecurringFor(recurringHfJob);
                }
                else
                {
                    JobProcessHelper.ExecuteOnceFor(baseHfJob);
                }
            }
        }
    }
}
