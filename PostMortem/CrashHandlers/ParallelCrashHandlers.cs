using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PostMortem.CrashHandlers.Base;

namespace PostMortem.CrashHandlers
{
    public class ParallelCrashHandlers : CompositeCrashHandlerBase
    {
        public override async Task<bool> HandleCrashAsync(ICrashContext crashContext, CancellationToken cancellationToken)
        {
            try
            {
                var results = await Task.WhenAll(CrashHandlers.Select(x => x.HandleCrashAsync(crashContext, cancellationToken)));
                return AlwaysContinue || results.All(x => x);
            }
            catch (Exception)
            {
                if (AlwaysContinue)
                    return true;
                throw;
            }
        }
    }
}