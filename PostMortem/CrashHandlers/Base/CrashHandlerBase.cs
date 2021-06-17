using System.Threading;
using System.Threading.Tasks;

namespace PostMortem.CrashHandlers.Base
{
    public abstract class CrashHandlerBase : ICrashHandler
    {
        public abstract bool HandleCrashImmediately(ICrashContext crashContext);
        public abstract Task<bool> HandleCrashAsync(ICrashContext crashContext, IReport report, CancellationToken cancellationToken);
    }
}