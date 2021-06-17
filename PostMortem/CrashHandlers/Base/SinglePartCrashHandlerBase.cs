using System.Threading;
using System.Threading.Tasks;
using PostMortem.Utils;

namespace PostMortem.CrashHandlers.Base
{
    public abstract class SinglePartCrashHandlerBase : CrashHandlerBase
    {
        public object PartId { get; set; }
        public string SuggestedFileName { get; private set; }
        public CrashPathProvider PathProvider { get; } = new CrashPathProvider();

        public override bool HandleCrashImmediately(ICrashContext crashContext)
        {
            if (!PathProvider.HasName)
                PathProvider.Name = GetDefaultFileName(crashContext);

            SuggestedFileName = PathProvider.GetName(crashContext);
            return true;
        }

        public override sealed async Task<bool> HandleCrashAsync(ICrashContext crashContext, IReport report, CancellationToken cancellationToken)
        {
            await CreatePartAsync(crashContext, report, cancellationToken);
            return true;
        }

        protected abstract string GetDefaultFileName(ICrashContext crashContext);
        protected abstract Task CreatePartAsync(ICrashContext crashContext, IReport report, CancellationToken cancellationToken);
    }
}