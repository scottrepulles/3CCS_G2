using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DHK.Module.BusinessObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHK.Module.Converters
{
    public class ImportMappingClassInfoTypeConverter : LocalizedClassInfoTypeConverter
    {
        public override List<Type> GetSourceCollection(ITypeDescriptorContext context)
        {
            return [
                typeof(Student),
               typeof(Teacher),
               typeof(Course),
            ];
        }
    }
}
