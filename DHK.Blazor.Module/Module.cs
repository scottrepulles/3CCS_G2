using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Xpo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DHK.Blazor.Module.BusinessObjects.Globals;
using System.Data;
using DHK.Module.Enumerations;
using DHK.Module.BusinessObjects;

namespace DHK.Blazor.Module;

// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
public sealed class ModuleModule : ModuleBase {

    private string connectionString = null;

    public ModuleModule() {
        AdditionalExportedTypes.Add(typeof(BaseHangfireJob));
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
    }
    public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
        ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
        return [updater];
    }
    public override void Setup(XafApplication application) {
        base.Setup(application);
        // Manage various aspects of the application UI and behavior at the module level.

        IConfiguration configuration = application.ServiceProvider.GetRequiredService<IConfiguration>();
        string environment = configuration["ASPNETCORE_ENVIRONMENT"];

        bool isLocalDeployment = ApplicationEnvironmentType.Development.ToString().Equals(environment, StringComparison.InvariantCultureIgnoreCase);  
        if (configuration != null)
            if (isLocalDeployment)
                connectionString = configuration["ConnectionStrings:LocalConnectionString"];
            else
                connectionString = configuration["ConnectionStrings:ConnectionString"];

        application.ObjectSpaceCreated += Application_ObjectSpaceCreated;
    }
    
    private void Application_ObjectSpaceCreated(object sender, ObjectSpaceCreatedEventArgs e)
    {
        CompositeObjectSpace compositeObjectSpace = e.ObjectSpace as CompositeObjectSpace;
        if (compositeObjectSpace != null)
        {
            if (!(compositeObjectSpace.Owner is CompositeObjectSpace))
            {
                compositeObjectSpace.PopulateAdditionalObjectSpaces((XafApplication)sender);
            }
        }
    }
}
