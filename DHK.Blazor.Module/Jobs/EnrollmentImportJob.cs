using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using DHK.Blazor.Module.BusinessObjects.Globals;
using DHK.Module.BusinessObjects;

namespace DHK.Blazor.Module.Jobs;

[MapInheritance(MapInheritanceType.ParentTable)]
[XafDisplayName(nameof(Enrollment))]
public class EnrollmentImportJob(Session session) : FileImportJob(session);
