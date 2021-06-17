using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PostMortem.CrashHandlers.Base
{
    public abstract class CompositeCrashHandlerBase : CrashHandlerBase, IEnumerable
    {
        public List<ICrashHandler> CrashHandlers { get; } = new List<ICrashHandler>();
        public bool AlwaysContinue { get; set; }

        public override bool HandleCrashImmediately(ICrashContext crashContext)
        {
            try
            {
                return CrashHandlers.All(x => x.HandleCrashImmediately(crashContext));
            }
            catch (Exception)
            {
                if (AlwaysContinue)
                    return true;
                throw;
            }
        }

        public override async Task<bool> HandleCrashAsync(ICrashContext crashContext, IReport report, CancellationToken cancellationToken)
        {
            try
            {
                return await HandleCrashAsync(x => x.HandleCrashAsync(crashContext, report, cancellationToken), AlwaysContinue);
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