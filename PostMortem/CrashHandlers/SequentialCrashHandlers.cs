using System;
using System.Threading;
using System.Threading.Tasks;
using PostMortem.CrashHandlers.Base;

namespace PostMortem.CrashHandlers
{
    public class SequentialCrashHandlers : CompositeCrashHandlerBase
    {
        protected override async Task<bool> HandleCrashAsync(Func<ICrashHandler, Task<bool>> taskGetter, bool alwaysContinue)
        {
            foreach (ICrashHandler crashHandler in CrashHandlers)
                if (!await taskGetter(crashHandler) && !alwaysContinue)
                    return false;

            return true;
        }

        public override async Task ConfigureReportAsync(IReport report, CancellationToken cancellationToken)
        {
            foreach (ICrashHandler crashHandler in CrashHandlers)
                await crashHandler.ConfigureReportAsync(report, cancellationToken);
        }
    }
}