using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DHK.Module.Constants;
using DHK.Module.Enumerations;
using DHK.Module.Enumerations;

namespace DHK.Blazor.Module.BusinessObjects.Globals;

[MapInheritance(MapInheritanceType.ParentTable)]
public abstract class HangfireImportJob(Session session) : BaseHangfireJob(session)
{
    ImportDuplicateRecordType importDuplicateRecordType;

    [XafDisplayName(DisplayNames.DUPLICATE_RECORD)]
    [VisibleInListView(false)]
    public ImportDuplicateRecordType ImportDuplicateRecordType
    {
        get => importDuplicateRecordType;
        set => SetPropertyValue(nameof(ImportDuplicateRecordType), ref importDuplicateRecordType, value);
    }
}
