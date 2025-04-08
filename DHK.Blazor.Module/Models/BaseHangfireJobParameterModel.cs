namespace DHK.Blazor.Module.Model;

public class BaseHangfireJobParameterModel
{
    public Guid ObjectId { get; set; } = default;
    public string RecurringId { get; set; } = null;
    public string ParentObjectOid { get; set; } = default;
    public string ParentObjecType { get; set; } = null;
    public string Mapping { get; set; } = null;
    public string TimeZone { get; set; } = null;
    public List<Guid> GuidList { get; set; }
    public Guid? MessageTemplate { get; set; }
    public Guid? EmailTemplate { get; set; }
    public string ActionType { get; set; }
    public string CurrentUser { get; set; }
    public string RepeatCode { get; set; }
    public string BaseCriteria { get; set; }
    public string RouteName { get; set; }
    public DateOnly? Date { get; set; }
    public Guid? ReportScheduleGuid { get; set; }
    public bool OrderRule { get; set; }
}
