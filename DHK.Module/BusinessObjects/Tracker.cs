using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace DHK.Module.BusinessObjects
{
    [DefaultClassOptions]
    public class Tracker(Session session) : CreationAuditedEntity(session)
    { 
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        Document document;
        string viewedBy;

        [Association($"{nameof(Tracker)}{nameof(Document)}")]   
        public Document Document
        {
            get => document;
            set => SetPropertyValue(nameof(Document), ref document, value);
        }

        public string ViewedBy
        {
            get => viewedBy;
            set => SetPropertyValue(nameof(ViewedBy), ref viewedBy, value);
        }
    }
}