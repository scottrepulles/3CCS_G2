using DevExpress.ExpressApp.MultiTenancy.Internal;
using DevExpress.ExpressApp.Security;
using DHK.Module.Enumerations;
using DHK.Module.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DHK.Blazor.Module.Helpers.Globals;

public static class ServiceCredentialsHelper
{
    public static void RunWithServiceCredentials(IServiceScope scope, System.Action action)
    {
        var configuration = scope.ServiceProvider?.GetRequiredService<IConfiguration>();
        var tenantNameHelper = scope.ServiceProvider?.GetRequiredService<ITenantNameHelper>();

        if (configuration != null)
        {
            var userName = configuration["Services:LocalUserName"];
            var ***REMOVED*** = configuration["Services:LocalPassword"];
            var tenantName = configuration["Services:LocalTenant"];

            string environment = configuration["ASPNETCORE_ENVIRONMENT"];
            bool isLocalDeployment = ApplicationEnvironmentType.Development.ToString().Equals(environment, StringComparison.InvariantCultureIgnoreCase);
            if (!isLocalDeployment)
            {
                userName = configuration["Services:UserName"];
                ***REMOVED*** = configuration["Services:Password"];
                tenantName = configuration["Services:Tenant"];
            }

            try
            {
                var tenantId = tenantNameHelper.GetTenantIdByName(tenantName);
            }
            catch (Exception)
            {
                tenantName = "";
            }

            var signInManager = scope.ServiceProvider?.GetRequiredService<SignInManager>();
            if (signInManager != null)
            {
                var authResult = signInManager.AuthenticateByLogonParameters(new MultiTenantLogonParametersModel
                {
                    Company = tenantName,
                    UserName = userName,
                    Password = ***REMOVED***
                });
                if (authResult != null)
                {
                    signInManager.SignInByPrincipal(authResult.Principal);
                }
            }
        }

        action.Invoke();
    }

    public static async Task RunWithServiceCredentialsAsync(IServiceScope scope, Func<Task> action)
    {
        var configuration = scope.ServiceProvider?.GetRequiredService<IConfiguration>();
        var tenantNameHelper = scope.ServiceProvider?.GetRequiredService<ITenantNameHelper>();

        if (configuration != null)
        {
            var userName = configuration["Services:LocalUserName"];
            var ***REMOVED*** = configuration["Services:LocalPassword"];
            var tenantName = configuration["Services:LocalTenant"];

            string environment = configuration["ASPNETCORE_ENVIRONMENT"];
            bool isLocalDeployment = ApplicationEnvironmentType.Development.ToString().Equals(environment, StringComparison.InvariantCultureIgnoreCase);
            if (!isLocalDeployment)
            {
                userName = configuration["Services:UserName"];
                ***REMOVED*** = configuration["Services:Password"];
                tenantName = configuration["Services:Tenant"];
            }

            try
            {
                var tenantId = tenantNameHelper.GetTenantIdByName(tenantName);
            }
            catch (Exception)
            {
                tenantName = "";
            }

            var signInManager = scope.ServiceProvider?.GetRequiredService<SignInManager>();
            if (signInManager != null)
            {
                var authResult = signInManager.AuthenticateByLogonParameters(new MultiTenantLogonParametersModel
                {
                    Company = tenantName,
                    UserName = userName,
                    Password = ***REMOVED***
                });
                if (authResult != null)
                {
                    signInManager.SignInByPrincipal(authResult.Principal);
                }
            }
        }

        await action.Invoke();
    }
}
