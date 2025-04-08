using DevExpress.Xpo;
using DHK.Module.BusinessObjects;
using DHK.Module.Models;

namespace DHK.Blazor.Module.BusinessObjects.Globals;

public class HangfireJobLogMessage(Session session) : AuditedEntity(session)
{
    HangfireJobLog hangfireJobLog;
    string message;

    [Size(SizeAttribute.Unlimited)]
    public string Message
    {
        get => message;
        set => SetPropertyValue(nameof(Message), ref message, value);
    }

    [Association($"{nameof(HangfireJobLogMessage)}{nameof(HangfireJobLog)}")]
    public HangfireJobLog HangfireJobLog
    {
        get => hangfireJobLog;
        set => SetPropertyValue(nameof(HangfireJobLog), ref hangfireJobLog, value);
    }
}
