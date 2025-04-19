using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using DHK.Blazor.Module.BusinessObjects.Globals;
using DHK.Module.BusinessObjects;

namespace DHK.Blazor.Module.Jobs;

[MapInheritance(MapInheritanceType.ParentTable)]
[XafDisplayName(nameof(College))]
public class CollegeImportJob(Session session) : FileImportJob(session);
