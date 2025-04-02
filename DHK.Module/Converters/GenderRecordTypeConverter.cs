using DevExpress.Xpo.Metadata;
using DKH.Module.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKH.Module.Converters
{
    public class GenderRecordTypeConverter : ValueConverter
    {
        public override Type StorageType
            => typeof(string);

        public override object ConvertFromStorageType(object value)
        {
            if ((string)value == GenderType.Male.ToString())
                return GenderType.Male;
            else if ((string)value == GenderType.Female.ToString())
                return GenderType.Female;
            else return null;
        }

        public override object ConvertToStorageType(object value)
        {
            return value == null ? null : ((GenderType)value).ToString();
        }
    }
}
