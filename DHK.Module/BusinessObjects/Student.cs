using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DHK.Module.Converters;
using DHK.Module.Enumerations;
using DHK.Module.Helper;
using DHK.Module.Interfaces;
using DKH.Module.Constants;
using DKH.Module.Converters;
using DKH.Module.Enumerations;
using DKH.Module.Interfaces;
using Microsoft.VisualBasic;
using System.ComponentModel;

namespace DHK.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DefaultProperty(nameof(FormattedFullName))]
    public class Student(Session session) : AuditedUser(session), IAuditEvent, IImported, IGeneratedIdentifier
    {
        string studentNumber;
        YearLevelType yearLevel;
        DateTime? birthday;
        DateTime? enrollmentDate;
        GenderType gender;
        string address;
        string middleName;
        string lastName;
        string firstName;
        Program program;

        protected override void OnSaving()
        {
            this.GenerateIdentifier(nameof(StudentNumber));
            RoleHelper.AddUserRole(this, Session, RoleNames.STUDENTS);
            base.OnSaving();
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

        [NonCloneable]
        [RuleUniqueValue]
        [Indexed(Unique = true)]
        [Size(50)]
        [ModelDefault(ModelDefaultProperties.PROPERTY_EDITOR_ALLOW_EDIT, ModelDefaultProperties.IS_FALSE)]
        public string StudentNumber
        {
            get => studentNumber;
            set => SetPropertyValue(nameof(StudentNumber), ref studentNumber, value);
        }

        [ValueConverter(typeof(GenericEnumConverter<YearLevelType>))]
        public YearLevelType YearLevel
        {
            get => yearLevel;
            set => SetPropertyValue(nameof(YearLevel), ref yearLevel, value);
        }

        public DateTime? Birthday
        {
            get => birthday;
            set => SetPropertyValue(nameof(Birthday), ref birthday, value);
        }

        [ValueConverter(typeof(GenderRecordTypeConverter))]
        public GenderType Gender
        {
            get => gender;
            set => SetPropertyValue(nameof(Gender), ref gender, value);
        }
        public string Address
        {
            get => address;
            set => SetPropertyValue(nameof(Address), ref address, value);
        }
        public DateTime? EnrollmentDate
        {
            get => enrollmentDate;
            set => SetPropertyValue(nameof(EnrollmentDate), ref enrollmentDate, value);
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

        public Program Program
        {
            get => program;
            set => SetPropertyValue(nameof(Program), ref program, value);
        }
    }
}
