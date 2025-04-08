using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DHK.Module.Constants;
using DKH.Module.Constants;

namespace DHK.Blazor.Module.BusinessObjects.Globals;

[DefaultClassOptions]
public class FtpServer : BaseObject
{
    public FtpServer(Session session)
    : base(session)
    {
    }

    public override void AfterConstruction()
    {
        base.AfterConstruction();
    }

    string serverName;
    string userName;
    string ***REMOVED***;
    string folderName;
    bool sftp;

    [RuleRequiredField(DefaultContexts.Save)]
    public string ServerName
    {
        get => serverName;
        set => SetPropertyValue(nameof(ServerName), ref serverName, value);
    }

    [RuleRequiredField(DefaultContexts.Save)]
    public string UserName
    {
        get => userName;
        set => SetPropertyValue(nameof(UserName), ref userName, value);
    }

    [RuleRequiredField(DefaultContexts.Save)]
    [ModelDefault(ModelDefaultProperties.IS_PASSWORD, ModelDefaultProperties.IS_TRUE)]
    public string Password
    {
        get => ***REMOVED***;
        set => SetPropertyValue(nameof(Password), ref ***REMOVED***, value);
    }

    public string FolderName
    {
        get => folderName;
        set => SetPropertyValue(nameof(FolderName), ref folderName, value);
    }

    public bool Sftp
    {
        get => sftp;
        set => SetPropertyValue(nameof(Sftp), ref sftp, value);
    }
}