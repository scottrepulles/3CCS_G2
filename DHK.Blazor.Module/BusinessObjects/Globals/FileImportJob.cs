using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DHK.Module.Constants;
using System.ComponentModel;

namespace DHK.Blazor.Module.BusinessObjects.Globals;

[MapInheritance(MapInheritanceType.ParentTable)]
public abstract class FileImportJob(Session session) : HangfireImportJob(session)
{
    FileData file;

    [ImmediatePostData]
    [RuleRequiredField(DefaultContexts.Save)]
    public FileData File
    {
        get => file;
        set => SetPropertyValue(nameof(File), ref file, value);
    }

    string parentObjectOid;
    [Browsable(false)]
    [Size(FieldSizes.NOTES)]
    public string ParentObjectOid
    {
        get => parentObjectOid;
        set => SetPropertyValue(nameof(ParentObjectOid), ref parentObjectOid, value);
    }

    string parentObjectType;
    [Browsable(false)]
    [Size(FieldSizes.NOTES)]
    public string ParentObjectType
    {
        get => parentObjectType;
        set => SetPropertyValue(nameof(ParentObjectType), ref parentObjectType, value);
    }
}
