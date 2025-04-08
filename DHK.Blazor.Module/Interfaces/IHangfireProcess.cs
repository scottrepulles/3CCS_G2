using Hangfire.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHK.Blazor.Module.Interfaces;

public interface IHangfireProcess
{
    void Execute(PerformContext performContext,
        string baseHangfireJobParameter = null);
}
