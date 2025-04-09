using DevExpress.ExpressApp.DC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHK.Module.Enumerations
{
    public enum YearLevelType
    {
        [XafDisplayName(@"1st Year")]
        FIRST = 1,
        [XafDisplayName(@"2nd Year")]
        SECOND = 2,
        [XafDisplayName(@"3rd Year")]
        THIRD = 3,
        [XafDisplayName(@"4th Year")]
        FOURTH = 4,
    }
}
