using DevExpress.Xpo;
using DHK.Blazor.Module.BusinessObjects.Globals;

namespace DHK.Blazor.Module.BusinessObjects.Schedules;

[MapInheritance(MapInheritanceType.ParentTable)]
public abstract class RecurringHangfireJob(Session session) : BaseHangfireJob(session)
{
    string recurringJobId;

    public string RecurringJobId
    {
        get => recurringJobId;
        set => SetPropertyValue(nameof(RecurringJobId), ref recurringJobId, value);
    }

    public abstract string GenerateJobId();

    public abstract string GetCron();
}
