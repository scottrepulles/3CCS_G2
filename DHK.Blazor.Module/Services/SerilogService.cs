using DevExpress.Persistent.Base;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHK.Blazor.Module.Services
{
    public class SerilogService : Tracing
    {
        public override void LogError(Exception exception)
        {
            Log.Logger.Error(exception, exception.Message);
        }

        public override void LogError(string text, params object[] args)
        {
            Log.Logger.Error(text, args);
        }

        public override void LogText(string text, params object[] args)
        {
            Log.Logger.Information(text, args);
        }

        public override void LogWarning(string text, params object[] args)
        {
            Log.Logger.Warning(text, args);
        }

        public override void LogVerboseError(Exception exception)
        {
            Log.Logger.Verbose(exception, exception.Message);
        }

        public override void LogVerboseText(string text, params object[] args)
        {
            Log.Logger.Verbose(text, args);
        }
    }
}
