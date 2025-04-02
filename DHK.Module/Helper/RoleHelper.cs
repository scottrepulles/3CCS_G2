using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Xpo;

namespace DHK.Module.Helper;

public static class RoleHelper
{
    public static void AddUserRole<T>(T obj, Session session, string roleName)
        where T : PermissionPolicyUser
    {
        if (session is not NestedUnitOfWork
                && session.DataLayer != null
                && session.IsNewObject(obj)
                && session.ObjectLayer is not SecuredSessionObjectLayer)
        {
            PermissionPolicyRole role = session.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse($"{nameof(PermissionPolicyRole.Name)} = ?", roleName));
            if (role != null)
            {
                obj.Roles.Add(role);
            }
        }
    }
}
