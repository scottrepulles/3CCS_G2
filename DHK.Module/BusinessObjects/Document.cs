using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DHK.Module.Interfaces;

namespace DHK.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DefaultProperty(nameof(Course.Title))]

    public class Document(Session session) : AuditedEntity(session), IImported, IAuditEvent
    {
        FileData file;
        Course course;
        bool visible;
        DateTime? expirationDate;


        [ImmediatePostData]
        [RuleRequiredField(DefaultContexts.Save)]
        public FileData File
        {
            get => file;
            set => SetPropertyValue(nameof(File), ref file, value);
        }

        IFileData viewer;
        [EditorAlias("DocumentViewEditor")]
        public IFileData Viewer
        {
            get
            {
                if (file != null)
                {
                    return file;
                }
                return viewer;
            }
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

        [Association($"{nameof(Course)}{nameof(Document)}")]
        public Course Course
        {
            get => course;
            set => SetPropertyValue(nameof(Course), ref course, value);
        }

        public bool Visible
        {
            get => visible;
            set => SetPropertyValue(nameof(Visible), ref visible, value);
        }
        public DateTime? ExpirationDate
        {
            get => expirationDate;
            set => SetPropertyValue(nameof(ExpirationDate), ref expirationDate, value);
        }
    }
}
