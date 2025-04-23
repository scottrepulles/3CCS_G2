using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DHK.Module.Converters;
using DHK.Module.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DHK.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DefaultProperty(nameof(Year))]
    public class AcademicYear(Session session) : AuditedEntity(session)
    {
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        string year;
        bool isCurrent;
        SemesterType semester;

        public string Year
        {
            get => year;
            set => SetPropertyValue(nameof(Year), ref year, value);
        }

        public bool IsCurrent
        {
            get => isCurrent;
            set => SetPropertyValue(nameof(IsCurrent), ref isCurrent, value);
        }

        [ValueConverter(typeof(GenericEnumConverter<SemesterType>))]
        public SemesterType Semester
        {
            get => semester;
            set => SetPropertyValue(nameof(Semester), ref semester, value);
        }
    }
}