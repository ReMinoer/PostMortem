using System;
using System.Threading;
using System.Threading.Tasks;
using PostMortem.CrashHandlers.Base;

namespace PostMortem.CrashHandlers
{
    public class SequentialCrashHandlers : CompositeCrashHandlerBase
    {
        public override async Task<bool> HandleCrashAsync(ICrashContext crashContext, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var crashHandler in CrashHandlers)
                    if (!await crashHandler.HandleCrashAsync(crashContext, cancellationToken) && !AlwaysContinue)
                        return false;

                return true;
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