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
        string IReportPart.SuggestedFileName => Path.GetFileName(FilePath);

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

        public async Task AddFileAsync(IReportFile reportFile, object partId, bool removeFile, CancellationToken cancellationToken)
        {
            using (IPartStream partStream = await CreatePartStreamAsync(reportFile, partId, cancellationToken))
            using (FileStream fileStream = File.OpenRead(reportFile.FilePath))
                await fileStream.CopyToAsync(partStream.Stream, 4096, CancellationToken.None);

            if (removeFile)
                File.Delete(reportFile.FilePath);
        }

        public async Task AddTextAsync(IReportText reportText, object partId, CancellationToken cancellationToken)
        {
            using (IPartStream partStream = await CreatePartStreamAsync(reportText, partId, cancellationToken))
            using (StreamWriter streamWriter = new StreamWriter(partStream.Stream))
                await streamWriter.WriteAsync(reportText.Text);
        }

        public async Task AddBytesAsync(IReportBytes reportBytes, object partId, CancellationToken cancellationToken)
        {
            using (IPartStream partStream = await CreatePartStreamAsync(reportBytes, partId, cancellationToken))
                await partStream.Stream.WriteAsync(reportBytes.Bytes, 0, reportBytes.Bytes.Length, cancellationToken);
        }

        public async Task<IPartStream> CreatePartStreamAsync(IReportPart reportPart, object partId, CancellationToken cancellationToken)
        {
            IDisposable writeLock = await WriteLockAsync(cancellationToken);
            Stream entryStream = _zipArchive.CreateEntry(reportPart.SuggestedFileName).Open();

            return new PartStream(entryStream, writeLock);
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