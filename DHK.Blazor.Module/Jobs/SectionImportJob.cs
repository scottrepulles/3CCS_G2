using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using DHK.Blazor.Module.BusinessObjects.Globals;
using DHK.Module.BusinessObjects;

namespace DHK.Blazor.Module.Jobs;

[MapInheritance(MapInheritanceType.ParentTable)]
[XafDisplayName(nameof(Section))]
public class SectionImportJob(Session session) : FileImportJob(session);
