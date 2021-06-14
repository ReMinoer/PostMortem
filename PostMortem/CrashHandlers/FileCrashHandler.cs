using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PostMortem.CrashHandlers.Base;

namespace PostMortem.CrashHandlers
{
    public class FileCrashHandler : CrashHandlerBase, IReportFile
    {
        public string FilePath { get; set; }
        public bool DeleteFile { get; set; }
        public bool CanReport => !string.IsNullOrWhiteSpace(FilePath) && File.Exists(FilePath);

        public FileCrashHandler()
        {
        }

        public FileCrashHandler(string filePath)
        {
            FilePath = filePath;
        }

        public override Task<bool> HandleCrashAsync(ICrashContext crashContext, CancellationToken cancellationToken) => Task.FromResult(true);

        public override async Task ConfigureReportAsync(IReport report, CancellationToken cancellationToken)
        {
            await report.AddFileAsync(this, DeleteFile, cancellationToken);
        }
    }
}