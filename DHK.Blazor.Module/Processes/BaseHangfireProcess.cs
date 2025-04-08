using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.Blazor.AmbientContext;
using DevExpress.ExpressApp.Blazor.Services;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.MultiTenancy.Internal;
using DevExpress.ExpressApp.Security;
using Hangfire.Logging;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DHK.Blazor.Module.BusinessObjects;
using DHK.Blazor.Module.Helpers;
using DHK.Blazor.Module.Helpers.Globals;
using DHK.Blazor.Module.Interfaces;
using DHK.Module.BusinessObjects;
using DHK.Module.Enumerations;
using DHK.Module.Interfaces.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHK.Module.Interfaces;

namespace DHK.Blazor.Module.Processes;

public abstract class BaseHangfireProcess : IHangfireProcess
{
    protected IServiceScopeFactory ServiceScopeFactory { get; }

    public BaseHangfireProcess() {}

    [ActivatorUtilitiesConstructor]
    public BaseHangfireProcess(IServiceScopeFactory serviceScopeFactory)
    {
        ServiceScopeFactory = serviceScopeFactory;
    }

    protected void RunActionUsing(Action<IServiceProvider, IObjectSpace> action)
    {
        using (IServiceScope scope = ServiceScopeFactory?.CreateScope())
        {
            ServiceCredentialsHelper.RunWithServiceCredentials(scope, () =>
            {
                IValueManagerStorageContext valueManagerContext = scope.ServiceProvider?.GetRequiredService<IValueManagerStorageContext>();
                valueManagerContext?.RunWithStorage(() =>
                {
                    valueManagerContext.EnsureStorage();

                    BlazorApplication application = scope.ServiceProvider?.GetRequiredService<IXafApplicationProvider>()
                                           ?.GetApplication();
                    using (IObjectSpace objectSpace = ((INonsecuredObjectSpaceProvider)application.ObjectSpaceProvider)?.CreateNonsecuredObjectSpace())
                    {
                        if (objectSpace is not null)
                        {
                            action(scope.ServiceProvider, objectSpace);
                        }
                        else throw new Exception("Cannot create objectSpace");
                    }
                });
            });
        }
    }

    protected D GetHfJobData<D>(IObjectSpace objectSpace, string jobId)
        where D : IHangfireJobData
    {
        return objectSpace.GetObjectsQuery<D>().FirstOrDefault(job => job.BackgroundJobId == jobId);
    }

    public abstract void Execute(PerformContext performContext,
        string baseHangfireJobParameter = null);

}
