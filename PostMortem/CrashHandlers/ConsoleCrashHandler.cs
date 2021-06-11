using System;
using System.Threading;
using System.Threading.Tasks;
using PostMortem.CrashHandlers.Base;

namespace PostMortem.CrashHandlers
{
    public class ConsoleCrashHandler : CrashHandlerBase
    {
        public override Task<bool> HandleCrashAsync(ICrashContext crashContext, CancellationToken cancellationToken)
        {
            Console.WriteLine(crashContext.Exception.ToString());
            return Task.FromResult(true);
        }
    }
}