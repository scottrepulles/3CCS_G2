using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHK.Module.Enumerations
{
    public enum ActionImportType
    {
        [ImageName("Action_CreateDashboard")]
        [XafDisplayName(@"Import")]
        Import,
        [ImageName("BO_Audit_ChangeHistory")]
        [XafDisplayName(@"Logs")]
        ImportLogs,
        [ImageName("Action_Export_ToXLSX")]
        [XafDisplayName(@"Template")]
        CreateTemplate
    }
}
