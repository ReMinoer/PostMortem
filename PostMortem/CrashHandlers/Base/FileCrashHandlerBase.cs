using System.Threading;
using System.Threading.Tasks;

namespace PostMortem.CrashHandlers.Base
{
    public abstract class FileCrashHandlerBase : SinglePartCrashHandlerBase
    {
        public string FilePath { get; private set; }
        protected abstract bool RemoveFile { get; }

        public override bool HandleCrashImmediately(ICrashContext crashContext)
        {
            if (!base.HandleCrashImmediately(crashContext))
                return false;

            FilePath = PathProvider.GetPath(crashContext);
            return true;
        }

        protected override sealed async Task CreatePartAsync(ICrashContext crashContext, IReport report, CancellationToken cancellationToken)
        {
            await WriteFileAsync(FilePath, crashContext, cancellationToken);
            await report.AddFilePartAsync(FilePath, PartId, RemoveFile, cancellationToken);
        }
        
        protected abstract Task WriteFileAsync(string filePath, ICrashContext crashContext, CancellationToken cancellationToken);
    }
}