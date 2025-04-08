using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.ApplicationBuilder;
using DevExpress.ExpressApp.Blazor.ApplicationBuilder;
using DevExpress.ExpressApp.Blazor.Services;
using DevExpress.Persistent.Base;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Server.Circuits;
using DevExpress.ExpressApp.Xpo;
using DHK.Blazor.Server.Services;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.ExpressApp.MultiTenancy;
using Microsoft.Extensions.DependencyInjection;
using DHK.Module.Enumerations;
using DHK.Module.BusinessObjects;
using DHK.Module;
using DHK.Module.Models;
using Hangfire;
using Hangfire.Console;
using DHK.Blazor.Module.Hangfire;

namespace DHK.Blazor.Server;

public class Startup {
    public Startup(IConfiguration configuration) {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services) {

        string environment = Configuration["ASPNETCORE_ENVIRONMENT"];
        string connectionString = null;

        bool isLocalDeployment = ApplicationEnvironmentType.Development.ToString().Equals(environment, StringComparison.InvariantCultureIgnoreCase);
        var tenantName = Configuration["Services:Tenant"];
        var hangfireConnectionString = Configuration["ConnectionStrings:TenantHangfire"];
        if (isLocalDeployment)
        {
            tenantName = Configuration["Services:LocalTenant"];
            hangfireConnectionString = Configuration["ConnectionStrings:LocalTenantHangfire"];
            connectionString = Configuration["ConnectionStrings:LocalConnectionString"];
        }
        else
        {
            connectionString = Configuration["ConnectionStrings:ConnectionString"];
        }
        bool isTenantInstance = !string.IsNullOrEmpty(tenantName);

        services.AddSingleton(typeof(Microsoft.AspNetCore.SignalR.HubConnectionHandler<>), typeof(ProxyHubConnectionHandler<>));

        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddHttpContextAccessor();
        services.AddScoped<CircuitHandler, CircuitHandlerProxy>();
        services.AddXaf(Configuration, builder =>
        {
            builder.UseApplication<DHKBlazorApplication>();
            builder.Modules
                .AddAuditTrailXpo()
                .AddCloningXpo()
                .AddConditionalAppearance()
                .AddDashboards(options =>
                {
                    options.DashboardDataType = typeof(DevExpress.Persistent.BaseImpl.DashboardData);
                })
                .AddFileAttachments()
                .AddNotifications()
                .AddOffice()
                .AddReports(options =>
                {
                    options.EnableInplaceReports = true;
                    options.ReportDataType = typeof(DevExpress.Persistent.BaseImpl.ReportDataV2);
                    options.ReportStoreMode = DevExpress.ExpressApp.ReportsV2.ReportStoreModes.XML;
                })
                .AddScheduler()
                .AddStateMachine(options =>
                {
                    options.StateMachineStorageType = typeof(DevExpress.ExpressApp.StateMachine.Xpo.XpoStateMachine);
                })
                .AddValidation(options =>
                {
                    options.AllowValidationDetailsAccess = false;
                })
                .AddViewVariants()
                .Add<DHK.Module.DHKModule>()    
                .Add<DHKBlazorModule>();

            builder.AddMultiTenancy()
                .WithCustomTenantType<BaseTenant>()
                .WithHostDatabaseConnectionString(connectionString)
                .WithMultiTenancyModelDifferenceStore(options =>
                {
#if !RELEASE
                    options.UseTenantSpecificModel = false;
#endif
                })
                .WithTenantResolver<BaseTenantResolver>();
            builder.ObjectSpaceProviders
                .AddSecuredXpo((serviceProvider, options) =>
                {
                    string multiTenantConnectionSting = serviceProvider.GetRequiredService<IConnectionStringProvider>()
                                                                       .GetConnectionString();
                    ArgumentNullException.ThrowIfNull(multiTenantConnectionSting);
                    options.ConnectionString = multiTenantConnectionSting;
                    options.ThreadSafe = true;
                    options.UseSharedDataStoreProvider = true;
                })
                .AddNonPersistent();
            builder.Security
                .UseIntegratedMode(options =>
                {
                    options.Lockout.Enabled = true;
                    options.RoleType = typeof(PermissionPolicyRole);
                    options.UserType = typeof(ApplicationUser);
                    options.UserLoginInfoType = typeof(ApplicationUserLoginInfo);
                    options.UseXpoPermissionsCaching();
                    options.Events.OnSecurityStrategyCreated += securityStrategy =>
                    {
                        ((SecurityStrategy)securityStrategy).PermissionsReloadMode = PermissionsReloadMode.NoCache;
                        ((SecurityStrategy)securityStrategy).AssociationPermissionsMode = AssociationPermissionsMode.Manual;
                    };
                })
                .AddPasswordAuthentication(options =>
                {
                    options.IsSupportChangePassword = true;
                    if (isLocalDeployment)
                    {
                        options.LogonParametersType = typeof(MultiTenantLogonParametersModel);
                    }
                    else
                    {
                        options.LogonParametersType = typeof(CustomLogonParametersForStandardAuthenticationModel);
                    }
                })
            .AddAuthenticationProvider<DKHAuthenticationProvider>();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/DKHLoginPage";
            });
        });
        services.AddHangfire((serviceProvider, config) =>
        {
            config
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(hangfireConnectionString)
                .UseConsole();
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
        bool isLocalDeployment = env.IsDevelopment();

        var tenantName = Configuration["Services:Tenant"];
        if (isLocalDeployment)
        {
            app.UseDeveloperExceptionPage();
            tenantName = Configuration["Services:LocalTenant"];
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. To change this for production scenarios, see: https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        bool isTenantInstance = !string.IsNullOrEmpty(tenantName);
        app.UseHttpsRedirection();
        app.UseRequestLocalization();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseXaf();
        app.UseEndpoints(endpoints => {
            endpoints.MapXafEndpoints();
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
            endpoints.MapControllers();
        });
        //Hangfire
        if (isTenantInstance)
        {
            app.UseHangfireDashboard(
                "/hangfire",
                new DashboardOptions
                {
                    Authorization = [new HangfireAuthFilter()]
                }
            );
        }
    }
}
