using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DHK.Module.BusinessObjects;
using System.Security.Claims;
using System.Security.Principal;

namespace DHK.Blazor.Server.Services
{
    public class DKHAuthenticationProvider : IAuthenticationProviderV2
    {
        private readonly IPrincipalProvider principalProvider;

        public DKHAuthenticationProvider(IPrincipalProvider principalProvider)
        {
            this.principalProvider = principalProvider;
        }

        public object Authenticate(IObjectSpace objectSpace)
        {
            if (!CanHandlePrincipal(principalProvider.User))
            {
                return null;
            }

            const bool autoCreateUser = true;

            ClaimsPrincipal claimsPrincipal = (ClaimsPrincipal)principalProvider.User;
            Claim userIdClaim = claimsPrincipal.FindFirst("sub") ?? claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("Unknown user id");

            string providerUserKey = userIdClaim.Value;
            string loginProviderName = claimsPrincipal.Identity.AuthenticationType;
            string userName = claimsPrincipal.Identity.Name;

            ISecurityUserLoginInfo userLoginInfo = FindUserLoginInfo(objectSpace, loginProviderName, providerUserKey);
            if (userLoginInfo != null)
            {
                return userLoginInfo.User;
            }

            if (autoCreateUser)
            {
                return CreateApplicationUser(objectSpace, userName, loginProviderName, providerUserKey);
            }

            // return null;
        }

        private bool CanHandlePrincipal(IPrincipal user)
        {
            return user.Identity.IsAuthenticated &&
                user.Identity.AuthenticationType != SecurityDefaults.Issuer &&
                user.Identity.AuthenticationType != SecurityDefaults.PasswordAuthentication &&
                user.Identity.AuthenticationType != SecurityDefaults.WindowsAuthentication &&
                !(user is WindowsPrincipal);
        }

        private object CreateApplicationUser(IObjectSpace objectSpace, string userName, string loginProviderName, string providerUserKey)
        {
            ApplicationUser user = objectSpace.CreateObject<ApplicationUser>();
            user.UserName = userName;
            user.SetPassword(Guid.NewGuid().ToString());
            user.Roles.Add(objectSpace.FirstOrDefault<PermissionPolicyRole>(role => role.Name == "Default"));
            ((ISecurityUserWithLoginInfo)user).CreateUserLoginInfo(loginProviderName, providerUserKey);
            objectSpace.CommitChanges();
            return user;
        }

        private ISecurityUserLoginInfo FindUserLoginInfo(IObjectSpace objectSpace, string loginProviderName, string providerUserKey)
        {
            return objectSpace.FirstOrDefault<ApplicationUserLoginInfo>(userLoginInfo =>
                                userLoginInfo.LoginProviderName == loginProviderName &&
                                userLoginInfo.ProviderUserKey == providerUserKey);
        }
    }
}
