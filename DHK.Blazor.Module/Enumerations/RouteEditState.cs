using DevExpress.ExpressApp.DC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHK.Blazor.Module.Enumerations;

public enum RouteEditState
{
    [XafDisplayName(@"Change Index Disabled")]
    Disabled,
    [XafDisplayName(@"Change Index Enabled")]
    Enabled
}
