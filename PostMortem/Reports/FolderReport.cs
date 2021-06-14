using System;
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

        public event EventHandler Reported;
        public event EventHandler Cancelled;

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

        public Task ReportAsync(CancellationToken cancellationToken)
        {
            Reported?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }

        public Task CancelAsync()
        {
            if (!string.IsNullOrEmpty(FolderPath) && Directory.Exists(FolderPath))
                Directory.Delete(FolderPath);

            Cancelled?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }

        public async Task AddFileAsync(IReportFile reportFile, bool removeFile, CancellationToken cancellationToken)
        {
            if (removeFile)
                await Task.Run(() => File.Move(reportFile.FilePath, Path.Combine(FolderPath, Path.GetFileName(reportFile.FilePath))), cancellationToken);
            else
                await Task.Run(() => File.Copy(reportFile.FilePath, Path.Combine(FolderPath, Path.GetFileName(reportFile.FilePath)), overwrite: true), cancellationToken);
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