using DHK.Module.Enumerations;

namespace DHK.Module.Interfaces;

public interface IHangfireJobData
{
    public string BackgroundJobId { get; set; }

    public HangfireJobStateType State { get; set; }

    public string JobDetails { get; }
}
