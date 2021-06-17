using System;
using System.Threading;
using System.Threading.Tasks;
using PostMortem.CrashHandlers.Base;

namespace PostMortem.CrashHandlers
{
    public class ConsoleCrashHandler : CrashHandlerBase
    {
        public override bool HandleCrashImmediately(ICrashContext crashContext)
        {
            Console.WriteLine(crashContext.Exception.ToString());
            return true;
        }

        public override Task<bool> HandleCrashAsync(ICrashContext crashContext, IReport report, CancellationToken cancellationToken) => Task.FromResult(true);
    }
}