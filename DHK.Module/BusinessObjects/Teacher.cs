using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DHK.Module.BusinessObjects;
using DHK.Module.Helper;
using DHK.Module.Interfaces;
using DKH.Module.Constants;
using DKH.Module.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DKH.Module.BusinessObjects
{
    [DefaultClassOptions]
    public class Teacher(Session session) : AuditedUser(session), IAuditEvent, IImported, IGeneratedIdentifier
    { 
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        protected override void OnSaving()
        {
            this.GenerateIdentifier(nameof(TeacherNumber));
            RoleHelper.AddUserRole(this, Session, RoleNames.STUDENTS);
            base.OnSaving();
        }

        DateTime? hireDate;
        string specialization;
        string teacherNumber;


        [NonCloneable]
        [RuleUniqueValue]
        [Indexed(Unique = true)]
        [Size(50)]
        [ModelDefault(ModelDefaultProperties.PROPERTY_EDITOR_ALLOW_EDIT, ModelDefaultProperties.IS_FALSE)]
        public string TeacherNumber
        {
            get => teacherNumber;
            set => SetPropertyValue(nameof(TeacherNumber), ref teacherNumber, value);
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

        [Association($"{nameof(Teacher)}{nameof(Course)}")]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        public XPCollection<Course> Courses
        {
            get
            {
                return GetCollection<Course>(nameof(Courses));
            }
        }
    }
}