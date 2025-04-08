using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DHK.Module.Constants;
using DKH.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHK.Module.BusinessObjects
{
    [NonPersistent]
    public abstract class AuditedType(Session session) : AuditedEntity(session)
    {
        string code;
        string description;

        [Size(FieldSizes.QUARTER)]
        [RuleRequiredField(DefaultContexts.Save)]
        public string Description
        {
            get => description;
            set => SetPropertyValue(nameof(Description), ref description, value);
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        [ModelDefault(ModelDefaultProperties.EDIT_MASK_TYPE, ModelDefaultProperties.REGEX)]
        [ModelDefault(ModelDefaultProperties.EDIT_MASK, Patterns.ALPHANUMERIC)]
        [VisibleInListView(false)]
        public string Code
        {
            get => code;
            set => SetPropertyValue(nameof(Code), ref code, value);
        }
    }
}
