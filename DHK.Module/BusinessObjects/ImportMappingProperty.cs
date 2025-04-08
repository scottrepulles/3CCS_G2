using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DHK.Module.Constants;
using DHK.Module.Converters;
using DHK.Module.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DHK.Module.BusinessObjects
{
    [DefaultProperty(nameof(PropertySelect))]
    public class ImportMappingProperty(Session session) : BaseObject(session)
    {
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        protected override void OnSaving()
        {
            if (Session is not NestedUnitOfWork
                && Session.DataLayer != null
                && Session.IsNewObject(this)
                && Session.ObjectLayer is not SecuredSessionObjectLayer
                && SortOrder == 0)
            {
                ImportMappingProperty maxSortEntity = Session.Query<ImportMappingProperty>()
                 .Where(e => e.ImportMapping == ImportMapping)
                 .OrderByDescending(e => e.SortOrder)
                 .FirstOrDefault();
                int count = 0;
                if (maxSortEntity != null) count = maxSortEntity.SortOrder;
                SortOrder = count + 1;
            }
        }



        [ValueConverter(typeof(TypeToStringConverter))]
        [TypeConverter(typeof(ImportMappingClassInfoTypeConverter))]
        [XafDisplayName(DisplayNames.ENTITY)]
        [ImmediatePostData]
        [Browsable(false)]
        public Type EntityDataType
        {
            get
            {
                if (ImportMapping != null)
                {
                    return ImportMapping.EntityDataType;
                }

                return null;
            }
        }
        string mapTo;
        string childrenProperty;
        string defaultValue;
        string sampleValue;
        bool required;
        private string property;
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        [XafDisplayName(DisplayNames.PROPERTY)]
        [ImmediatePostData]
        [Browsable(false)]
        public string Property
        {
            get => property;
            set
            {
                SetPropertyValue(nameof(Property), ref property, value);
            }
        }

        string propertyType;
        [Browsable(false)]
        public string PropertyType
        {
            get => propertyType;
            set => SetPropertyValue(nameof(PropertyType), ref propertyType, value);
        }

        string childrenPropertyType;
        [Browsable(false)]
        public string ChildrenPropertyType
        {
            get => childrenPropertyType;
            set => SetPropertyValue(nameof(ChildrenPropertyType), ref childrenPropertyType, value);
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        [XafDisplayName(DisplayNames.CHILDREN_PROPERTY)]
        [ImmediatePostData]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [Browsable(false)]
        public string ChildrenProperty
        {
            get => childrenProperty;
            set => SetPropertyValue(nameof(ChildrenProperty), ref childrenProperty, value);
        }


        Type parentPropertype;
        [Browsable(false)]
        public Type ParentPropertyType
        {
            get => parentPropertype;
            set => SetPropertyValue(nameof(ParentPropertyType), ref parentPropertype, value);
        }

        int sortOrder;
        [Browsable(false)]
        public int SortOrder
        {
            get => sortOrder;
            set => SetPropertyValue(nameof(SortOrder), ref sortOrder, value);
        }

        ImportMapping importMapping;
        [Association($"{nameof(ImportMapping)}{nameof(ImportMappingProperty)}")]
        public ImportMapping ImportMapping
        {
            get => importMapping;
            set => SetPropertyValue(nameof(ImportMapping), ref importMapping, value);
        }

        [CollectionOperationSet(AllowAdd = false, AllowRemove = false)]
        [Browsable(false)]
        public XPCollection<AuditDataItemPersistent> AuditEvents
        {
            get
            {
                return GetCollection<AuditDataItemPersistent>(nameof(AuditEvents));
            }
        }

        [Browsable(false)]
        [RuleFromBoolProperty(nameof(ValidateProperty), DefaultContexts.Save, CustomMessages.PROPERTY_EXISTS, UsedProperties = DisplayNames.PROPERTY)]
        public bool ValidateProperty
        {
            get
            {
                List<ImportMappingProperty> importMappingDetails = ImportMapping.Properties.Where(s => s.EntityDataType.Equals(EntityDataType)
                && s.Property == Property
                && s.ChildrenProperty == ChildrenProperty).ToList();

                if (importMappingDetails.Count > 1)
                {
                    return false;
                }
                return true;
            }
        }


        PropertyModel propertySelect;
        [XafDisplayName(DisplayNames.PROPERTY)]
        [DataSourceProperty(nameof(PropertyList))]
        [ImmediatePostData]
        [NonPersistent]
        public PropertyModel PropertySelect
        {
            get
            {
                if (!IsLoading && !IsSaving)
                {
                    PropertyModel pModel = PropertyList.Where(x => x.PropertyName == Property).FirstOrDefault();
                    if (pModel?.ParentPropertyType != null)
                    {
                        ITypeInfo typeInfo1 = XafTypesInfo.Instance.FindTypeInfo(pModel?.ParentPropertyType);
                        childrenPropertyList = new List<PropertyModel>();
                        ParentPropertyType = pModel.ParentPropertyType;
                        foreach (IMemberInfo memberInfo1 in typeInfo1.Members)
                        {
                            MappableAttribute mappable = memberInfo1.FindAttribute<MappableAttribute>();
                            if (memberInfo1.IsVisible || mappable != null)
                            {
                                PropertyModel add = new PropertyModel();
                                add.PropertyName = memberInfo1.Name;
                                add.PropertyType = memberInfo1.MemberTypeInfo.Type.FullName;
                                if (memberInfo1.MemberTypeInfo.Base?.Type == typeof(object)
                                    || memberInfo1.MemberTypeInfo.Base?.Type == typeof(ValueType)
                                    || memberInfo1.MemberTypeInfo.Base?.Type == typeof(BaseObject))
                                {
                                    childrenPropertyList.Add(add);
                                }
                            }
                        }
                    }
                    return PropertyList.Where(x => x.PropertyName == Property).FirstOrDefault();
                }
                return propertySelect;
            }
            set
            {
                bool modified = SetPropertyValue(nameof(PropertySelect), ref propertySelect, value);
                if (!IsLoading && !IsSaving && modified)
                {
                    if (value != null && EntityDataType != null)
                    {
                        ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(EntityDataType);
                        IMemberInfo memberinfo = typeInfo.FindMember(value?.PropertyName);
                        ITypeInfo memberType = memberinfo.MemberTypeInfo;
                        PropertyType = memberType.UnderlyingTypeInfo == null ? memberType.Type.FullName : memberType.UnderlyingTypeInfo.FullName;
                        Property = memberinfo.Name;

                        MapTo = memberinfo.Name;

                        ParentPropertyType = value?.ParentPropertyType;
                        if (value?.ParentPropertyType != null)
                        {
                            childrenPropertyList = new List<PropertyModel>();
                            ITypeInfo typeInfo1 = XafTypesInfo.Instance.FindTypeInfo(value?.ParentPropertyType);
                            foreach (IMemberInfo memberInfo1 in typeInfo1.Members)
                            {
                                MappableAttribute mappable = memberInfo1.FindAttribute<MappableAttribute>();
                                if (memberInfo1.IsVisible || mappable != null)
                                {
                                    PropertyModel add = new PropertyModel();
                                    add.PropertyName = memberInfo1.Name;
                                    add.PropertyType = memberInfo1.MemberTypeInfo.Type.FullName;
                                    if (memberInfo1.MemberTypeInfo.Base?.Type == typeof(object)
                                        || memberInfo1.MemberTypeInfo.Base?.Type == typeof(ValueType)
                                        || memberInfo1.MemberTypeInfo.Base?.Type == typeof(BaseObject))
                                    {
                                        childrenPropertyList.Add(add);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        [Browsable(false)]
        private List<PropertyModel> PropertyList
        {
            get
            {
                List<PropertyModel> list = new List<PropertyModel>();
                if (EntityDataType != null)
                {
                    ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(EntityDataType);
                    foreach (IMemberInfo memberInfo in typeInfo.Members)
                    {

                        MappableAttribute mappable = memberInfo.FindAttribute<MappableAttribute>();
                        if (memberInfo.IsVisible || mappable != null)
                        {
                            PropertyModel add = new PropertyModel();
                            add.PropertyName = memberInfo.Name;
                            add.PropertyType = memberInfo.MemberTypeInfo.Type.FullName;
                            if (memberInfo.MemberTypeInfo.Base != null)
                            {
                                if (memberInfo.MemberTypeInfo.Base.Type == typeof(object)
                                || memberInfo.MemberTypeInfo.Base.Type == typeof(ValueType)
                                || memberInfo.MemberTypeInfo.Base.Type == typeof(BaseObject)
                                || memberInfo.MemberTypeInfo.Type == typeof(Country))
                                {
                                    list.Add(add);
                                }
                                else
                                {
                                    if (memberInfo.MemberTypeInfo.Base.Type != typeof(XPBaseCollection))
                                    {
                                        add.ParentPropertyType = memberInfo.MemberTypeInfo.Type;
                                        list.Add(add);
                                    }
                                }
                            }
                            else
                            {
                                if (memberInfo.MemberTypeInfo.Type == typeof(Country))
                                {
                                    list.Add(add);
                                }
                            }
                        }
                    }
                }
                return list;
            }
        }


        PropertyModel childrenPropertySelect;
        [XafDisplayName(DisplayNames.CHILDREN_PROPERTY)]
        [DataSourceProperty(nameof(ChidrenPropertyList))]
        [ImmediatePostData]
        [NonPersistent]
        [VisibleInListView(true)]
        public PropertyModel ChildrenPropertySelect
        {
            get
            {
                if (!IsLoading && !IsSaving && childrenPropertyList != null)
                {
                    return childrenPropertyList.Where(x => x.PropertyName == ChildrenProperty).FirstOrDefault();
                }
                return null;
            }
            set
            {
                if (!IsLoading && !IsSaving)
                {
                    if (value != null && ParentPropertyType != null)
                    {
                        ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(ParentPropertyType);
                        IMemberInfo memberinfo = typeInfo.FindMember(value?.PropertyName);
                        ITypeInfo memberType = memberinfo.MemberTypeInfo;
                        ChildrenProperty = value?.PropertyName;
                        ChildrenPropertyType = memberType.UnderlyingTypeInfo == null ? memberType.Type.FullName : memberType.UnderlyingTypeInfo.FullName;

                        MapTo = $"{Property}.{value?.PropertyName}";

                        SetPropertyValue(nameof(ChildrenPropertySelect), ref childrenPropertySelect, value);
                    }
                }
            }
        }
        List<PropertyModel> childrenPropertyList = new List<PropertyModel>();
        [Browsable(false)]
        [ImmediatePostData]
        public List<PropertyModel> ChidrenPropertyList
        {
            get
            {
                if (ParentPropertyType != null)
                {
                    childrenPropertyList = new List<PropertyModel>();
                    ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(ParentPropertyType);
                    foreach (IMemberInfo memberInfo in typeInfo.Members)
                    {
                        MappableAttribute mappable = memberInfo.FindAttribute<MappableAttribute>();
                        if (memberInfo.IsVisible || mappable != null)
                        {
                            PropertyModel add = new PropertyModel();
                            add.PropertyName = memberInfo.Name;
                            add.PropertyType = memberInfo.MemberTypeInfo.Type.FullName;
                            if (memberInfo.MemberTypeInfo.Base.Type == typeof(object)
                                || memberInfo.MemberTypeInfo.Base.Type == typeof(ValueType)
                                || memberInfo.MemberTypeInfo.Base.Type == typeof(BaseObject))
                            {
                                childrenPropertyList.Add(add);
                            }
                        }
                    }
                }
                return childrenPropertyList;
            }
        }

        [Appearance(nameof(HideChildrenProperty), TargetItems = nameof(ChildrenPropertySelect)
            , Visibility = ViewItemVisibility.Hide, Context = "DetailView")]
        public bool HideChildrenProperty()
        {
            return ParentPropertyType == null;
        }
        public bool Required
        {
            get => required;
            set => SetPropertyValue(nameof(Required), ref required, value);
        }
        public string MapTo
        {
            get => mapTo;
            set => SetPropertyValue(nameof(MapTo), ref mapTo, value);
        }
        public string DefaultValue
        {
            get => defaultValue;
            set => SetPropertyValue(nameof(DefaultValue), ref defaultValue, value);
        }

        public string SampleValue
        {
            get => sampleValue;
            set => SetPropertyValue(nameof(SampleValue), ref sampleValue, value);
        }
    }
}