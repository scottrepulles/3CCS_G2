using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DHK.Module.Converters;
using DHK.Module.Enumerations;
using DKH.Module.Constants;
using DKH.Module.Interfaces;
using System.ComponentModel;

namespace DHK.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DefaultProperty(nameof(Title))]
    public class Course(Session session) : AuditedEntity(session), IImported, IAuditEvent
    {
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        string code;
        string title;
        Program program;
        YearLevelType yearLevel;

        [NonCloneable]
        [RuleUniqueValue]
        [Indexed(Unique = true)]
        [Size(50)]
        [ModelDefault(ModelDefaultProperties.EDIT_MASK_TYPE, Patterns.REGEX)]
        [ModelDefault(ModelDefaultProperties.EDIT_MASK, Patterns.ALPHANUMERIC)]
        [ModelDefault(ModelDefaultProperties.PROPERTY_EDITOR_ALLOW_EDIT, ModelDefaultProperties.IS_FALSE)]
        public string Code
        {
            get => code;
            set => SetPropertyValue(nameof(Code), ref code, value);
        }

        [RuleRequiredField(DefaultContexts.Save)]
        public string Title
        {
            get => title;
            set => SetPropertyValue(nameof(Title), ref title, value);
        }

        [Association($"{nameof(Program)}{nameof(Course)}")]
        public Program Program
        {
            get => program;
            set => SetPropertyValue(nameof(Program), ref program, value);
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

        [Association($"{nameof(Teacher)}{nameof(Course)}")]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        public XPCollection<Teacher> Teachers
        {
            get
            {
                return GetCollection<Teacher>(nameof(Teachers));
            }
        }

        [ValueConverter(typeof(GenericEnumConverter<YearLevelType>))]
        public YearLevelType YearLevel
        {
            get => yearLevel;
            set => SetPropertyValue(nameof(YearLevel), ref yearLevel, value);
        }

        [Association($"{nameof(Course)}{nameof(Document)}")]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        public XPCollection<Document> Documents
        {
            get
            {
                return GetCollection<Document>(nameof(Documents));
            }
        }
    }
}