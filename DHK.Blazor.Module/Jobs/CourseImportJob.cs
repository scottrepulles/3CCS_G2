using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using DHK.Blazor.Module.BusinessObjects.Globals;
using DHK.Module.BusinessObjects;

namespace DHK.Blazor.Module.Jobs;

[MapInheritance(MapInheritanceType.ParentTable)]
[XafDisplayName(nameof(Course))]
public class CourseImportJob(Session session) : FileImportJob(session);