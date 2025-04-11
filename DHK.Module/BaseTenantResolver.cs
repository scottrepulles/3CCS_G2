using DevExpress.ExpressApp.MultiTenancy.Internal;
using DevExpress.ExpressApp.MultiTenancy;
using DevExpress.ExpressApp.Security;
using DHK.Module.Enumerations;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHK.Module.Constants;
using DHK.Module.Models;

namespace DHK.Module
{
    public class BaseTenantResolver : ITenantResolver
    {
        readonly ITenantNameHelper tenantNameHelper;
        readonly ISecurityStrategyBase security;
        readonly IConfiguration configuration;

        public BaseTenantResolver(
            ITenantNameHelper tenantNameHelper,
            ISecurityStrategyBase security,
            IConfiguration configuration
        )
        {
            this.tenantNameHelper = tenantNameHelper;
            this.security = security;
            this.configuration = configuration;
        }

        public string FormatUserLogin(string userLogin, string tenantName)
        {
            var formattedLogin = $"{tenantName}.{userLogin}";
            return formattedLogin;
        }

        public Guid? GetTenantId(IAuthenticationStandardLogonParameters parameters)
        {
            if (parameters is MultiTenantLogonParametersModel multiTenantLogonParameters)
            {
                var parametersTenantName = AppConfigVariables.TENANT_NAME;
                Console.WriteLine($"BaseTenantResolver tenant = {parametersTenantName}");
                if (!string.IsNullOrEmpty(parametersTenantName))
                {
                    return TenantIdByName(parametersTenantName);
                }
            }

            //Do not get the tenant name from appsettings when running in local development mode
            if (!isLocalDeployment())
            {
                var environmentTenantName = GetTenantNameFromConfig();
                if (!string.IsNullOrEmpty(environmentTenantName))
                {
                    Console.WriteLine($"BaseTenantResolver tenant = {environmentTenantName}");
                    return TenantIdByName(environmentTenantName);
                }
            }

            Console.WriteLine($"BaseTenantResolver tenant = null");
            return null;
        }

        public Guid? GetTenantId(string userLogin)
        {
            if (security.LogonParameters is MultiTenantLogonParametersModel multiTenantLogonParameters)
            {
                var parametersTenantName = AppConfigVariables.TENANT_NAME;
                Console.WriteLine($"BaseTenantResolver tenant = {parametersTenantName}");
                if (!string.IsNullOrEmpty(parametersTenantName))
                {
                    return TenantIdByName(parametersTenantName);
                }
            }

            //Do not get the tenant name from appsettings when running in local development mode
            if (!isLocalDeployment())
            {
                var environmentTenantName = GetTenantNameFromConfig();
                if (!string.IsNullOrEmpty(environmentTenantName))
                {
                    Console.WriteLine($"BaseTenantResolver tenant = {environmentTenantName}");
                    return TenantIdByName(environmentTenantName);
                }
            }

            Console.WriteLine($"BaseTenantResolver tenant =  null");
            return null;
        }

        private Guid? TenantIdByName(string name)
        {
            try
            {
                return tenantNameHelper.GetTenantIdByName(name);
            }
            catch (Exception) { }
            return null;
        }

        public Guid? GetTenantIdFromConfig()
        {
            return TenantIdByName(GetTenantNameFromConfig());
        }

        public Guid? GetLoginTenantId()
        {
            if (security.LogonParameters is MultiTenantLogonParametersModel)
            {
                //string tenantName = "cict";
                string tenantName = "cict";
                if (!string.IsNullOrEmpty(tenantName))
                {
                    return TenantIdByName(tenantName);
                }
            }
            return null;
        }

        private bool isLocalDeployment()
        {
            string environment = configuration[AppConfigVariables.ASPNETCORE_ENVIRONMENT];
            return ApplicationEnvironmentType.Development.ToString().Equals(environment, StringComparison.InvariantCultureIgnoreCase);
        }

        private string GetTenantNameFromConfig()
        {
            var tenantName = configuration[AppConfigVariables.SERVICE_LOCAL_TENANT];

            if (!isLocalDeployment())
            {
                tenantName = configuration[AppConfigVariables.SERVICE_TENANT];
            }

            return tenantName;
        }
    }
}
