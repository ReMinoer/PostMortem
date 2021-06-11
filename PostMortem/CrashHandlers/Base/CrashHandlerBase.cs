using System.Threading;
using System.Threading.Tasks;

namespace PostMortem.CrashHandlers.Base
{
    public abstract class CrashHandlerBase : ICrashHandler
    {
        public abstract Task<bool> HandleCrashAsync(ICrashContext crashContext, CancellationToken cancellationToken);
        public virtual Task ConfigureReportAsync(IReport compositeReport, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}