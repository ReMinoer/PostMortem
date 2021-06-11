using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PostMortem.CrashHandlers.Base;

namespace PostMortem.Logging
{
    public class LogCrashHandler : CrashHandlerBase
    {
        private readonly ILogger _logger;

        public LogCrashHandler(ILogger logger)
        {
            _logger = logger;
        }

        public override Task<bool> HandleCrashAsync(ICrashContext crashContext, CancellationToken cancellationToken)
        {
            if (crashContext.Unhandled)
                _logger.LogCritical(crashContext.Exception, null);
            else
                _logger.LogError(crashContext.Exception, null);

            return Task.FromResult(true);
        }
    }
}