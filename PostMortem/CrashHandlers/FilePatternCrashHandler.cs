using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PostMortem.CrashHandlers.Base;

namespace PostMortem.CrashHandlers
{
    public class FilePatternCrashHandler : CrashHandlerBase
    {
        public string FolderPath { get; set; }
        public string FileNamePattern { get; set; }
        public object PartId { get; set; }
        public bool DeleteFiles { get; set; }

        public FilePatternCrashHandler()
        {
            FolderPath = Directory.GetCurrentDirectory();
        }

        public FilePatternCrashHandler(string fileNamePattern)
            : this()
        {
            FileNamePattern = fileNamePattern;
        }

        public FilePatternCrashHandler(string folderPath, string fileNamePattern)
        {
            FolderPath = folderPath;
            FileNamePattern = fileNamePattern;
        }

        public override bool HandleCrashImmediately(ICrashContext crashContext) => true;

        public override async Task<bool> HandleCrashAsync(ICrashContext crashContext, IReport report, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Task.WhenAll(Directory.GetFiles(FolderPath, FileNamePattern).Select(x => report.AddFilePartAsync(x, PartId, DeleteFiles, cancellationToken)));
            return true;
        }

        public override Task CleanAfterCancelAsync() => Task.CompletedTask;
    }
}