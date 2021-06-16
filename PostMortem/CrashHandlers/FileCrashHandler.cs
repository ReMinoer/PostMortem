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
        public object PartId { get; set; }

        public string SuggestedFileName => Path.GetFileName(FilePath);
        public bool CanReport => !string.IsNullOrWhiteSpace(FilePath) && File.Exists(FilePath);

        public FileCrashHandler()
        {
        }

        public FileCrashHandler(string filePath)
        {
            FilePath = filePath;
        }

        public override async Task<bool> HandleCrashAsync(ICrashContext crashContext, IReport report, CancellationToken cancellationToken)
        {
            await report.AddFileAsync(this, PartId, DeleteFile, cancellationToken);
            return true;
        }
    }
}