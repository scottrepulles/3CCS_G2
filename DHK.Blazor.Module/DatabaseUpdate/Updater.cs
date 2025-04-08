using DevExpress.ExpressApp;
using DevExpress.ExpressApp.MultiTenancy;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base;
using Microsoft.Extensions.DependencyInjection;
using DHK.Blazor.Module.BusinessObjects.Globals;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DKH.Module.Constants;

namespace DHK.Blazor.Module.DatabaseUpdate;

public class Updater(IObjectSpace objectSpace, Version currentDBVersion) : ModuleUpdater(objectSpace, currentDBVersion)
{
    string TenantName
        => ObjectSpace.ServiceProvider.GetRequiredService<ITenantProvider>().TenantName;

    public override void UpdateDatabaseAfterUpdateSchema()
    {
        base.UpdateDatabaseAfterUpdateSchema();

        if (TenantName != null)
        {
            SeedTenanUsersAndRoles();
        }
    }

    void SeedTenanUsersAndRoles()
    {
        try
        {
            UpdateStatus("UpdateManagerRole", string.Empty, "Updating manager role in the database...");
            UpdateManagerRole();
        }
        catch (Exception e)
        {
            Tracing.Tracer.LogText("Cannot initialize default data from the XML files.");
            Tracing.Tracer.LogError(e);
        }
        ObjectSpace.CommitChanges();
    }

    void UpdateManagerRole()
    {
        PermissionPolicyRole role = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(r => r.Name == RoleNames.TEACHERS);
        if (role == null)
            return;
        SetBackgroundRolePermissions(role);
    }

    static void SetBackgroundRolePermissions(PermissionPolicyRole role)
    {
        role.SetTypePermission<BaseHangfireJob>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
        role.SetTypePermission<FileImportJob>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
        role.SetTypePermission<HangfireImportJob>(SecurityOperations.Read + SecurityOperations.Delimiter + SecurityOperations.Create, SecurityPermissionState.Allow);
    }
}
