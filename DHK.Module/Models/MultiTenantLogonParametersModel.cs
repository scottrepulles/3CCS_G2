using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DKH.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHK.Module.Models
{
    [DomainComponent]
    [XafDisplayName("LOGIN")]
    public class MultiTenantLogonParametersModel : CustomLogonParametersForStandardAuthenticationModel
    {
        [ModelDefault(ModelDefaultProperties.NULL_TEXT, "Company")]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public string Company { get; set; }
    }
}
