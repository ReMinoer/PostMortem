using System.Threading;
using System.Threading.Tasks;
using PostMortem.Utils;

namespace PostMortem.CrashHandlers.Base
{
    public abstract class SinglePartCrashHandlerBase : CrashHandlerBase, IReportPart
    {
        public abstract bool CanReport { get; }
        public CrashPathProvider PathProvider { get; } = new CrashPathProvider();

        public override sealed async Task<bool> HandleCrashAsync(ICrashContext crashContext, CancellationToken cancellationToken)
        {
            if (!PathProvider.HasName)
                PathProvider.Name = GetDefaultFileName(crashContext);

            await CreatePartAsync(PathProvider, crashContext, cancellationToken);
            return true;
        }

        protected abstract string GetDefaultFileName(ICrashContext crashContext);
        protected abstract Task CreatePartAsync(CrashPathProvider pathProvider, ICrashContext crashContext, CancellationToken cancellationToken);
    }
}