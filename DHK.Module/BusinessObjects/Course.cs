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
using DHK.Module.Interfaces;
using DKH.Module.Constants;
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

        int unit;
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
        [XafDisplayName("Subject Description")]
        public string Title
        {
            get => title;
            set => SetPropertyValue(nameof(Title), ref title, value);
        }

        public int Unit
        {
            get => unit;
            set => SetPropertyValue(nameof(Unit), ref unit, value);
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

        [ValueConverter(typeof(GenericEnumConverter<YearLevelType>))]
        public YearLevelType YearLevel
        {
            get => yearLevel;
            set => SetPropertyValue(nameof(YearLevel), ref yearLevel, value);
        }

        [Association($"{nameof(Course)}{nameof(Syllabus)}")]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        public XPCollection<Syllabus> Syllabus
        {
            get
            {
                return GetCollection<Syllabus>(nameof(Syllabus));
            }
        }

        [Association($"{nameof(Course)}{nameof(Section)}")]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        public XPCollection<Section> Sections
        {
            get
            {
                return GetCollection<Section>(nameof(Sections));
            }
        }
    }
}