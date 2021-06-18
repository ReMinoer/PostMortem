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
            cancellationToken.ThrowIfCancellationRequested();

            FolderPath = FolderPathProvider.GetPath(crashContext);

            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);

            return Task.CompletedTask;
        }

        public Task ReportAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Reported?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }

        public Task CleanAfterCancelAsync()
        {
            if (!string.IsNullOrEmpty(FolderPath) && Directory.Exists(FolderPath))
                Directory.Delete(FolderPath, recursive: true);

            Cancelled?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }

        public async Task AddFilePartAsync(string filePath, object partId, bool removeFile, CancellationToken cancellationToken)
        {
            if (removeFile)
                await Task.Run(() => File.Move(filePath, Path.Combine(FolderPath, Path.GetFileName(filePath))), cancellationToken);
            else
                await Task.Run(() => File.Copy(filePath, Path.Combine(FolderPath, Path.GetFileName(filePath)), overwrite: true), cancellationToken);
        }

        public async Task AddTextPartAsync(string text, string suggestedFileName, object partId, CancellationToken cancellationToken)
        {
            using (IReportPart reportPart = await CreatePartAsync(suggestedFileName, partId, cancellationToken))
            using (var streamWriter = new StreamWriter(reportPart.Stream))
                await streamWriter.WriteAsync(text);
        }

        public async Task AddBytesPartAsync(byte[] bytes, string suggestedFileName, object partId, CancellationToken cancellationToken)
        {
            using (IReportPart reportPart = await CreatePartAsync(suggestedFileName, partId, cancellationToken))
                await reportPart.Stream.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
        }

        public Task<IReportPart> CreatePartAsync(string suggestedFileName, object partId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            FileStream fileStream = File.Create(Path.Combine(FolderPath, suggestedFileName));
            IReportPart reportPart = new ReportPart(fileStream);

            return Task.FromResult(reportPart);
        }
    }
}