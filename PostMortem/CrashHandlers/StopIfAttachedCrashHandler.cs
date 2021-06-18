using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using PostMortem.CrashHandlers.Base;

namespace PostMortem.CrashHandlers
{
    public class StopIfAttachedCrashHandler : CrashHandlerBase
    {
        public override bool HandleCrashImmediately(ICrashContext crashContext)
        {
            return !(crashContext.Unhandled && Debugger.IsAttached);
        }

        public override Task<bool> HandleCrashAsync(ICrashContext crashContext, IReport report, CancellationToken cancellationToken) => Task.FromResult(true);
        public override Task CleanAfterCancelAsync() => Task.CompletedTask;
    }
}