using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp;

namespace DHK.Module.Models
{
    [DomainComponent]
    public class PropertyModel : NonPersistentBaseObject
    {
        public PropertyModel()
        {
        }

        string propertyType;
        Type parentPropertyType;
        string propertyName;

        public string PropertyName
        {
            get => propertyName;
            set => SetPropertyValue(ref propertyName, value);
        }

        public string PropertyType
        {
            get => propertyType;
            set => SetPropertyValue(ref propertyType, value);
        }
        public Type ParentPropertyType
        {
            get => parentPropertyType;
            set => SetPropertyValue(ref parentPropertyType, value);
        }

        private List<PropertyModel> _relatedObjects;

        public List<PropertyModel> SubPropertyModel
        {
            get
            {
                if (_relatedObjects == null)
                {
                    _relatedObjects = new List<PropertyModel>();
                }
                return _relatedObjects;
            }
        }
    }
}
