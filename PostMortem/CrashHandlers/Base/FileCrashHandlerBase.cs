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

        protected override sealed Task CreatePartAsync(CrashPathProvider pathProvider, ICrashContext crashContext, CancellationToken cancellationToken)
        {
            FilePath = pathProvider.GetPath(crashContext);
            return WriteFileAsync(FilePath, crashContext, cancellationToken);
        }

        public override async Task ConfigureReportAsync(IReport report, CancellationToken cancellationToken)
        {
            await report.AddFileAsync(this, RemoveFileOnReport, cancellationToken);
        }
        
        protected abstract Task WriteFileAsync(string filePath, ICrashContext crashContext, CancellationToken cancellationToken);
    }
}