using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.ApplicationBuilder;
using DevExpress.ExpressApp.Blazor.ApplicationBuilder;
using DevExpress.ExpressApp.Blazor.Services;
using DevExpress.Persistent.Base;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Server.Circuits;
using DHK.Blazor.Hangfire.Services;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using Hangfire;
using Hangfire.Console;
using DevExpress.Utils;
using DevExpress.ExpressApp.MultiTenancy;
using Serilog;
using DHK.Blazor.Module.Services;
using DHK.Module.Enumerations;
using DHK.Blazor.Module.Hangfire;
using DevExpress.ExpressApp.StateMachine.Xpo;
using DevExpress.Persistent.BaseImpl.MultiTenancy;
using DHK.Module;
using DHK.Module.BusinessObjects;
using DHK.Module.Models;

namespace DHK.Blazor.Hangfire;

public class Startup {
    public Startup(IConfiguration configuration) {
        Configuration = configuration;
    }

    public void ConfigureLogging(IServiceCollection services)
    {
        // Add serilog logging
        services.AddSerilog();

        // Integrate serilog to XAF
        Tracing.CreateCustomTracer += delegate (object s, CreateCustomTracerEventArgs args)
        {
            args.Tracer = new SerilogService();
        };
        Tracing.Initialize();
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services) {
        // Configure custom logging using Serilog
        ConfigureLogging(services);

        string environment = Configuration["ASPNETCORE_ENVIRONMENT"];
        string connectionString = null;

        bool isLocalDeployment = ApplicationEnvironmentType.Development.ToString().Equals(environment, StringComparison.InvariantCultureIgnoreCase);
        var tenantName = Configuration["Services:Tenant"];
        var hangfireConnectionString = Configuration["ConnectionStrings:TenantHangfire"];
        if (isLocalDeployment)
        {
            connectionString = Configuration["ConnectionStrings:LocalConnectionString"];
            tenantName = Configuration["Services:LocalTenant"];
            hangfireConnectionString = Configuration["ConnectionStrings:LocalTenantHangfire"];
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
        //services.AddScoped<ISprintPlacesClientLoggingService, SprintTekGeocoderLoggingService>();

        if (isTenantInstance) 
        {
            services.AddSingleton<HangfireJobDataBinder>();
        }
        
        services.AddXaf(Configuration, builder => {
            builder.UseApplication<HangfireBlazorApplication>();
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
                .AddStateMachine(options => {
                    options.StateMachineStorageType = typeof(XpoStateMachine);
                })
                .AddValidation(options => {
                    options.AllowValidationDetailsAccess = false;
                })
                .AddViewVariants()
                .Add<DHK.Module.DHKModule>()
                .Add<DHK.Blazor.Module.ModuleModule>()
                .Add<HangfireBlazorModule>();

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
                .AddSecuredXpo((serviceProvider, options) => {
                    string multiTenantConnectionSting = serviceProvider.GetRequiredService<IConnectionStringProvider>()
                                                   .GetConnectionString();
                    ArgumentNullException.ThrowIfNull(multiTenantConnectionSting);
                    options.ConnectionString = multiTenantConnectionSting;
                    options.ThreadSafe = true;
                    options.UseSharedDataStoreProvider = true;
                })
                .AddNonPersistent();
            builder.Security
                .UseIntegratedMode(options => {
                    options.Lockout.Enabled = true;

                    options.RoleType = typeof(PermissionPolicyRole);
                    // ApplicationUser descends from PermissionPolicyUser and supports the OAuth authentication. For more information, refer to the following topic: https://docs.devexpress.com/eXpressAppFramework/402197
                    // If your application uses PermissionPolicyUser or a custom user type, set the UserType property as follows:
                    options.UserType = typeof(ApplicationUser);
                    // ApplicationUserLoginInfo is only necessary for applications that use the ApplicationUser user type.
                    // If you use PermissionPolicyUser or a custom user type, comment out the following line:
                    options.UserLoginInfoType = typeof(ApplicationUserLoginInfo);
                    options.UseXpoPermissionsCaching();
                    options.Events.OnSecurityStrategyCreated += securityStrategy => {
                        // Use the 'PermissionsReloadMode.NoCache' option to load the most recent permissions from the database once
                        // for every Session instance when secured data is accessed through this instance for the first time.
                        // Use the 'PermissionsReloadMode.CacheOnFirstAccess' option to reduce the number of database queries.
                        // In this case, permission requests are loaded and cached when secured data is accessed for the first time
                        // and used until the current user logs out.
                        // See the following article for more details: https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Security.SecurityStrategy.PermissionsReloadMode.
                        ((SecurityStrategy)securityStrategy).PermissionsReloadMode = PermissionsReloadMode.NoCache;
                    };
                })
                .AddPasswordAuthentication(options => {
                    options.IsSupportChangePassword = true;
                    //options.LogonParametersType = typeof(CustomLogonParametersForStandardAuthentication);

                    if (isLocalDeployment)
                    {
                        options.LogonParametersType = typeof(MultiTenantLogonParametersModel);
                    }
                    else
                    {
                        options.LogonParametersType = typeof(CustomLogonParametersForStandardAuthenticationModel);
                    }
                });
        });
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => {
            options.LoginPath = "/LoginPage";
                });

        if (isTenantInstance) 
        {
            services.AddHangfire(config =>
            {
                config
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(hangfireConnectionString)
                    .UseMaxArgumentSizeToRender(1000)
                    .UseConsole();
        });

            services.AddHangfireServer((serviceProvider, options) =>
            {

        });
    }
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
        bool isLocalDeployment = env.IsDevelopment();
        bool isHostInstance = !string.IsNullOrEmpty(Configuration["Services:Tenant"]);

        AzureCompatibility.Enable = true;
        var tenantName = Configuration["Services:Tenant"];
        if (isLocalDeployment) {
            app.UseDeveloperExceptionPage();
            tenantName = Configuration["Services:LocalTenant"];
        }
        else {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. To change this for production scenarios, see: https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseRequestLocalization();
        app.UseStaticFiles();
        app.UseSerilogRequestLogging();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        bool isTenantInstance = !string.IsNullOrEmpty(tenantName);

        //Hangfire
        if (isTenantInstance)
        {
            app.UseHangfireDashboard(
                "/hangfire",
                new DashboardOptions
                {
                    Authorization = new[] { new HangfireAuthFilter() }
                }
            );
        }

        app.UseXaf();
        app.UseEndpoints(endpoints => {
            endpoints.MapXafEndpoints();
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
            endpoints.MapControllers();
            //Hangfire
            if (isTenantInstance)
            {
                endpoints.MapHangfireDashboard();
            }
        });

        if (isTenantInstance)
        {
            GlobalJobFilters.Filters.Add(app.ApplicationServices.GetRequiredService<HangfireJobDataBinder>());
        }
    }
}
