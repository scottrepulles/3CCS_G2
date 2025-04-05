using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;

namespace DHK.Module.Models;

[DomainComponent]
[XafDisplayName("Login")]
public class CustomLogonParametersForStandardAuthenticationModel : AuthenticationStandardLogonParameters, IServiceProviderConsumer
{
    private IServiceProvider serviceProvider;

    public void SetServiceProvider(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

}
