using System.Threading;
using System.Threading.Tasks;

namespace PostMortem.CrashHandlers.Base
{
    public abstract class CrashHandlerBase : ICrashHandler
    {
        public abstract Task<bool> HandleCrashAsync(ICrashContext crashContext, CancellationToken cancellationToken);
        public abstract Task ConfigureReportAsync(IReport report, CancellationToken cancellationToken);

        public virtual async Task<bool> HandleCrashAndConfigureReportAsync(ICrashContext crashContext, IReport report, CancellationToken cancellationToken)
        {
            if (!await HandleCrashAsync(crashContext, cancellationToken))
                return false;

            await ConfigureReportAsync(report, cancellationToken);
            return true;
        }
    }
}