using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace DHK.Module.Interfaces
{
    public interface IAuditEvent
    {
        XPCollection<AuditDataItemPersistent> AuditEvents { get; }
    }
}
