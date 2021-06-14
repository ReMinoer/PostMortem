using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PostMortem.CrashHandlers.Base
{
    public abstract class CompositeCrashHandlerBase : CrashHandlerBase, IEnumerable
    {
        public List<ICrashHandler> CrashHandlers { get; } = new List<ICrashHandler>();
        public bool AlwaysContinue { get; set; }

        public override Task<bool> HandleCrashAsync(ICrashContext crashContext, CancellationToken cancellationToken)
            => HandleCrashAsync(x => x.HandleCrashAsync(crashContext, cancellationToken));
        public override Task<bool> HandleCrashAndConfigureReportAsync(ICrashContext crashContext, IReport report, CancellationToken cancellationToken)
            => HandleCrashAsync(x => x.HandleCrashAndConfigureReportAsync(crashContext, report, cancellationToken));

        private async Task<bool> HandleCrashAsync(Func<ICrashHandler, Task<bool>> taskGetter)
        {
            try
            {
                return await HandleCrashAsync(taskGetter, AlwaysContinue);
            }
            catch (Exception)
            {
                if (AlwaysContinue)
                    return true;
                throw;
            }
        }

        protected abstract Task<bool> HandleCrashAsync(Func<ICrashHandler, Task<bool>> taskGetter, bool alwaysContinue);

        public void Add(ICrashHandler crashHandler) => CrashHandlers.Add(crashHandler);
        public IEnumerator GetEnumerator() => CrashHandlers.GetEnumerator();
    }
}