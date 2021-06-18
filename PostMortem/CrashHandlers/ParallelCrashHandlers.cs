using System;
using System.Linq;
using System.Threading.Tasks;
using PostMortem.CrashHandlers.Base;

namespace PostMortem.CrashHandlers
{
    public class ParallelCrashHandlers : CompositeCrashHandlerBase
    {
        protected override async Task<bool> HandleCrashAsync(Func<ICrashHandler, Task<bool>> taskGetter, bool alwaysContinue)
        {
            bool[] results = await Task.WhenAll(CrashHandlers.Select(taskGetter));
            return alwaysContinue || results.All(x => x);
        }

        public override Task CleanAfterCancelAsync()
        {
            return Task.WhenAll(CrashHandlers.Select(x => x.CleanAfterCancelAsync()));
        }
    }
}