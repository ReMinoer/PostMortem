using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PostMortem.Utils;

namespace PostMortem.Reports
{
    public class FolderReport : IReport
    {
        public string FolderPath { get; private set; }
        public CrashPathProvider FolderPathProvider { get; }
        public bool MoveFiles { get; set; } = true;

        public FolderReport()
        {
            FolderPathProvider = new CrashPathProvider
            {
                NameProvider = c => c.GetDefaultFolderName("crash_report")
            };
        }

        public Task PrepareAsync(ICrashContext crashContext, CancellationToken cancellationToken)
        {
            FolderPath = FolderPathProvider.GetPath(crashContext);

            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);
            return Task.CompletedTask;
        }

        public Task ProcessAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task AddFileAsync(IReportFile reportFile, CancellationToken cancellationToken)
        {
            if (MoveFiles)
                await Task.Run(() => File.Move(reportFile.FilePath, Path.Combine(FolderPath, Path.GetFileName(reportFile.FilePath))));
            else
                await Task.Run(() => File.Copy(reportFile.FilePath, Path.Combine(FolderPath, Path.GetFileName(reportFile.FilePath)), overwrite: true));
        }

        public async Task AddTextAsync(IReportText reportText, CancellationToken cancellationToken)
        {
            using (StreamWriter streamWriter = File.CreateText(Path.Combine(FolderPath, reportText.SuggestedFileName)))
            {
                await streamWriter.WriteAsync(reportText.Text);
            }
        }

        public async Task AddBytesAsync(IReportBytes reportBytes, CancellationToken cancellationToken)
        {
            using (FileStream fileStream = File.Create(Path.Combine(FolderPath, reportBytes.SuggestedFileName)))
            {
                await fileStream.WriteAsync(reportBytes.Bytes, 0, reportBytes.Bytes.Length, cancellationToken);
            }
        }
    }
}