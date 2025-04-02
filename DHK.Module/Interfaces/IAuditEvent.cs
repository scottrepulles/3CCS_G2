using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKH.Module.Interfaces
{
    public interface IAuditEvent
    {
        XPCollection<AuditDataItemPersistent> AuditEvents { get; }
    }
}
