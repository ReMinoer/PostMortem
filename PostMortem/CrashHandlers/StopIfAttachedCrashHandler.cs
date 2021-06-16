using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using PostMortem.CrashHandlers.Base;

namespace PostMortem.CrashHandlers
{
    public class StopIfAttachedCrashHandler : CrashHandlerBase
    {
        public override Task<bool> HandleCrashAsync(ICrashContext crashContext, IReport report, CancellationToken cancellationToken)
        {
            return Task.FromResult(!(crashContext.Unhandled && Debugger.IsAttached));
        }
    }
}