using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DHK.Module.Constants;
using DHK.Module.Converters;
using DHK.Module.Enumerations;
using DHK.Module.Helper;
using DHK.Module.Interfaces;
using Limilabs.Client.IMAP;
using System.ComponentModel;

namespace DHK.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DefaultProperty(nameof(FormattedFullName))]
    public class Teacher(Session session) : AuditedUser(session), IAuditEvent, IImported, IGeneratedIdentifier
    {
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        protected override void OnSaving()
        {
            this.GenerateIdentifier(nameof(EmployeeNumber));
            RoleHelper.AddUserRole(this, Session, RoleNames.STUDENTS);
            base.OnSaving();
        }

        DateTime? hireDate;
        string specialization;
        string employeeNumber;
        string middleName;
        string lastName;
        string firstName;
        EmploymentStatusType status;

        [NonCloneable]
        [RuleUniqueValue]
        [Indexed(Unique = true)]
        [Size(50)]
        [ModelDefault(ModelDefaultProperties.PROPERTY_EDITOR_ALLOW_EDIT, ModelDefaultProperties.IS_FALSE)]
        public string EmployeeNumber
        {
            get => employeeNumber;
            set => SetPropertyValue(nameof(EmployeeNumber), ref employeeNumber, value);
        }

        [VisibleInListView(false)]
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        [RuleRequiredField(DefaultContexts.Save)]
        public string FirstName
        {
            get => firstName;
            set => SetPropertyValue(nameof(FirstName), ref firstName, value);
        }

        [VisibleInListView(false)]
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        [RuleRequiredField(DefaultContexts.Save)]
        public string LastName
        {
            get => lastName;
            set => SetPropertyValue(nameof(LastName), ref lastName, value);
        }

        [VisibleInListView(false)]
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string MiddleName
        {
            get => middleName;
            set => SetPropertyValue(nameof(MiddleName), ref middleName, value);
        }

        [VisibleInListView(true)]
        [VisibleInDetailView(false)]
        [XafDisplayName("")]
        [PersistentAlias($"CONCAT([{nameof(FirstName)}], ' ', [{nameof(LastName)}])")]
        public string FormattedFullName
        {
            get => EvaluateAlias(nameof(FormattedFullName))?.ToString() ?? string.Empty;
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

        public DateTime? HireDate
        {
            get => hireDate;
            set => SetPropertyValue(nameof(HireDate), ref hireDate, value);
        }

        public string Specialization
        {
            get => specialization;
            set => SetPropertyValue(nameof(Specialization), ref specialization, value);
        }

        [Association($"{nameof(Teacher)}{nameof(Section)}")]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        public XPCollection<Section> Sections
        {
            get
            {
                return GetCollection<Section>(nameof(Sections));
            }
        }

        [ValueConverter(typeof(GenericEnumConverter<EmploymentStatusType>))]
        public EmploymentStatusType Status
        {
            get => status;
            set => SetPropertyValue(nameof(Status), ref status, value);
        }
    }
}