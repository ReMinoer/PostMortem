using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PostMortem.CrashHandlers.Base;

namespace PostMortem.CrashHandlers
{
    public class FilePatternCrashHandler : CrashHandlerBase
    {
        public string FolderPath { get; set; }
        public string FileNamePattern { get; set; }
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

        public override async Task<bool> HandleCrashAsync(ICrashContext crashContext, IReport report, CancellationToken cancellationToken)
        {
            foreach (string filePath in Directory.GetFiles(FolderPath, FileNamePattern))
                await report.AddFileAsync(new MatchingFile(filePath), DeleteFiles, cancellationToken);

            return true;
        }

        public class MatchingFile : IReportFile
        {
            public string FilePath { get; }
            public bool CanReport => !string.IsNullOrWhiteSpace(FilePath) && File.Exists(FilePath);

            public MatchingFile(string filePath)
            {
                FilePath = filePath;
            }
        }
    }
}