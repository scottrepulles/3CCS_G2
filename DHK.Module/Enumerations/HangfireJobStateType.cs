namespace DHK.Module.Enumerations;

public enum HangfireJobStateType
{
    Unassigned,
    Awaiting,
    Deleted,
    Enqueued,
    Failed,
    Processing,
    Scheduled,
    Succeeded
}
