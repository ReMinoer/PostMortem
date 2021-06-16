using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using PostMortem.Utils;

namespace PostMortem.Reports
{
    public class ZipArchiveReport : IReport, IReportFile
    {
        private FileStream _fileStream;
        private ZipArchive _zipArchive;

        public string FilePath { get; private set; }
        public CrashPathProvider PathProvider { get; }

        public bool CanReport => !string.IsNullOrWhiteSpace(FilePath) && File.Exists(FilePath);

        public event EventHandler Reported;
        public event EventHandler Cancelled;

        public ZipArchiveReport()
        {
            PathProvider = new CrashPathProvider
            {
                NameProvider = c => c.GetDefaultFileName("crash_report", "zip")
            };
        }

        public Task PrepareAsync(ICrashContext crashContext, CancellationToken cancellationToken)
        {
            FilePath = PathProvider.GetPath(crashContext);

            _fileStream = File.Create(FilePath);
            _zipArchive = new ZipArchive(_fileStream, ZipArchiveMode.Create);

            return Task.CompletedTask;
        }

        public async Task AddFileAsync(IReportFile reportFile, bool removeFile, CancellationToken cancellationToken)
        {
            using (await WriteLockAsync(cancellationToken))
            using (Stream entryStream = _zipArchive.CreateEntry(Path.GetFileName(reportFile.FilePath)).Open())
            using (FileStream fileStream = File.OpenRead(reportFile.FilePath))
                await fileStream.CopyToAsync(entryStream, 4096, CancellationToken.None);
        }

        public async Task AddTextAsync(IReportText reportText, CancellationToken cancellationToken)
        {
            using (await WriteLockAsync(cancellationToken))
            using (Stream entryStream = _zipArchive.CreateEntry(reportText.SuggestedFileName).Open())
            using (StreamWriter streamWriter = new StreamWriter(entryStream))
                await streamWriter.WriteAsync(reportText.Text);
        }

        public async Task AddBytesAsync(IReportBytes reportBytes, CancellationToken cancellationToken)
        {
            using (await WriteLockAsync(cancellationToken))
            using (Stream entryStream = _zipArchive.CreateEntry(reportBytes.SuggestedFileName).Open())
                await entryStream.WriteAsync(reportBytes.Bytes, 0, reportBytes.Bytes.Length, cancellationToken);
        }

        public Task ReportAsync(CancellationToken cancellationToken)
        {
            _zipArchive.Dispose();
            _fileStream.Dispose();

            return Task.CompletedTask;
        }

        public Task CancelAsync()
        {
            _zipArchive?.Dispose();
            _fileStream?.Dispose();

            if (CanReport)
                File.Delete(FilePath);

            Cancelled?.Invoke(this, EventArgs.Empty);

            return Task.CompletedTask;
        }

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        private async Task<IDisposable> WriteLockAsync(CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync(cancellationToken);
            return new LockDisposer(_semaphore);
        }

        private class LockDisposer : IDisposable
        {
            private readonly SemaphoreSlim _semaphore;

            public LockDisposer(SemaphoreSlim semaphore)
            {
                _semaphore = semaphore;
            }

            public void Dispose()
            {
                _semaphore.Release();
            }
        }
    }
}