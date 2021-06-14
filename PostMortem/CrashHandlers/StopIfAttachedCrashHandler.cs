using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using PostMortem.CrashHandlers.Base;

namespace PostMortem.CrashHandlers
{
    public class StopIfAttachedCrashHandler : CrashHandlerBase
    {
        public override Task<bool> HandleCrashAsync(ICrashContext crashContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(!(crashContext.Unhandled && Debugger.IsAttached));
        }

        public override Task ConfigureReportAsync(IReport report, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}