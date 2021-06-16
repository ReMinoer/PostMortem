using System.Threading;
using System.Threading.Tasks;
using PostMortem.Utils;

namespace PostMortem.CrashHandlers.Base
{
    public abstract class FileCrashHandlerBase : SinglePartCrashHandlerBase
    {
        public string FilePath { get; private set; }
        protected abstract bool RemoveFile { get; }

        protected override sealed async Task CreatePartAsync(CrashPathProvider pathProvider, ICrashContext crashContext, IReport report, CancellationToken cancellationToken)
        {
            FilePath = pathProvider.GetPath(crashContext);
            await WriteFileAsync(FilePath, crashContext, cancellationToken);
            await report.AddFilePartAsync(FilePath, PartId, RemoveFile, cancellationToken);
        }
        
        protected abstract Task WriteFileAsync(string filePath, ICrashContext crashContext, CancellationToken cancellationToken);
    }
}