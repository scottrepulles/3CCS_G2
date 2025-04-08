using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHK.Blazor.Module.Hangfire;

public class HangfireAuthFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context)
    {
        HttpContext httpContext = context.GetHttpContext();
        bool isAuthorized = httpContext?.User?.Identity?.IsAuthenticated ?? false;
        if (!isAuthorized)
        {
            httpContext?.Response.Redirect("/LoginPage?ReturnUrl=/hangfire");
        }
        return true;
    }
}
