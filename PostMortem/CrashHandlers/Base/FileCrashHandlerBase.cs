using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PostMortem.Utils;

namespace PostMortem.CrashHandlers.Base
{
    public abstract class FileCrashHandlerBase : SinglePartCrashHandlerBase, IReportFile
    {
        public string FilePath { get; private set; }
        protected abstract bool RemoveFileOnReport { get; }
        public override bool CanReport => !string.IsNullOrWhiteSpace(FilePath) && File.Exists(FilePath);

        protected override sealed async Task CreatePartAsync(CrashPathProvider pathProvider, ICrashContext crashContext, IReport report, CancellationToken cancellationToken)
        {
            FilePath = pathProvider.GetPath(crashContext);
            await WriteFileAsync(FilePath, crashContext, cancellationToken);
            await report.AddFileAsync(this, PartId, RemoveFileOnReport, cancellationToken);
        }
        
        protected abstract Task WriteFileAsync(string filePath, ICrashContext crashContext, CancellationToken cancellationToken);
    }
}