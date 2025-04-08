using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using Hangfire.Console;
using Hangfire.Server;
using DHK.Blazor.Module.BusinessObjects.Globals;
using DHK.Module.Enumerations;

namespace DHK.Blazor.Module.Helpers.Globals;

public static class AutomationLogs
{
    public static void Save(PerformContext performContext, string recurringId, IObjectSpace objectSpace, HangfireJobStateType jobState)
    {
        string jobId = performContext.BackgroundJob.Id;
        HangfireJobLog hfJobData = objectSpace.FindObject<HangfireJobLog>(CriteriaOperator.Parse(
                               "BackgroundJobId == ?",
                               jobId
                           ));
        if (hfJobData == null)
        {
            HangfireJobLog automationJob = objectSpace.CreateObject<HangfireJobLog>();
            automationJob.BackgroundJobId = jobId;
            automationJob.State = jobState;
            automationJob.BackgroundJobName = recurringId;
        }
        else {
            hfJobData.State = jobState;
        }

        objectSpace.CommitChanges();
    }

    public static void SaveLogs(PerformContext performContext, IObjectSpace objectSpace, List<HangfireJobLogMessage> messages)
    {
        string jobId = performContext.BackgroundJob.Id;
        HangfireJobLog hfJobData = objectSpace.FindObject<HangfireJobLog>(CriteriaOperator.Parse(
                               "BackgroundJobId == ?",
                               jobId
                           ));
        hfJobData?.JobLogs.AddRange(messages);
        objectSpace.CommitChanges();
    }

    public static void HangfireLogMessage(string message, PerformContext performContext, ConsoleTextColor color)
    {
        performContext.SetTextColor(color);
        performContext.WriteLine(message);
        performContext.ResetTextColor();
    }
}
