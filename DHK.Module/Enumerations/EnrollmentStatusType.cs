using DevExpress.ExpressApp.DC;

namespace DHK.Module.Enumerations;

public enum EnrollmentStatusType
{
    [XafDisplayName(@"Completed")]
    Completed,
    [XafDisplayName(@"Enrolled")]
    ENROLLED,
    [XafDisplayName(@"Not Taken")]
    NOTTAKEN,
}
