using DevExpress.Data.Filtering;
using DevExpress.DataAccess.DataFederation;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DHK.Module.Constants;
using DHK.Module.Interfaces;
using DKH.Module.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DHK.Module.BusinessObjects
{
    [DefaultClassOptions]
    public class Syllabus(Session session) : AuditedEntity(session), IAuditEvent, IImported
    {
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        string name;
        string content;
        string objective;
        Course course;
        AcademicYear academicYear;

        [RuleRequiredField(DefaultContexts.Save)]
        public string Name
        {
            get => name;
            set => SetPropertyValue(nameof(Name), ref name, value);
        }

        [Association($"{nameof(Course)}{nameof(Syllabus)}")]
        public Course Course
        {
            get => course;
            set => SetPropertyValue(nameof(Course), ref course, value);
        }
        public AcademicYear AcademicYear
        {
            get => academicYear;
            set => SetPropertyValue(nameof(AcademicYear), ref academicYear, value);
        }
        public string Content
        {
            get => content;
            set => SetPropertyValue(nameof(Content), ref content, value);
        }
        public string Objective
        {
            get => objective;
            set => SetPropertyValue(nameof(Objective), ref objective, value);
        }
        [Association($"{nameof(Syllabus)}{nameof(Document)}")]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        public XPCollection<Document> Documents
        {
            get
            {
                return GetCollection<Document>(nameof(Documents));
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
    }
}