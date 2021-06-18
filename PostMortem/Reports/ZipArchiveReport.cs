using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PostMortem.Utils;

namespace PostMortem.Reports
{
    public class ZipArchiveReport : IReport
    {
        private FileStream _fileStream;
        private ZipArchive _zipArchive;

        public string FilePath { get; private set; }
        public CrashPathProvider PathProvider { get; }

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
            cancellationToken.ThrowIfCancellationRequested();

            FilePath = PathProvider.GetPath(crashContext);

            _fileStream = File.Create(FilePath);
            _zipArchive = new ZipArchive(_fileStream, ZipArchiveMode.Update);

            return Task.CompletedTask;
        }

        public async Task AddFilePartAsync(string filePath, object partId, bool removeFile, CancellationToken cancellationToken)
        {
            using (IReportPart reportPart = await CreatePartAsync(Path.GetFileName(filePath), partId, cancellationToken))
            using (FileStream fileStream = File.OpenRead(filePath))
                await fileStream.CopyToAsync(reportPart.Stream, 4096, cancellationToken);

            if (removeFile)
                File.Delete(filePath);
        }

        public async Task AddTextPartAsync(string text, string suggestedFileName, object partId, CancellationToken cancellationToken)
        {
            using (IReportPart reportPart = await CreatePartAsync(suggestedFileName, partId, cancellationToken))
            using (StreamWriter streamWriter = new StreamWriter(reportPart.Stream))
                await streamWriter.WriteAsync(text);
        }

        public async Task AddBytesPartAsync(byte[] bytes, string suggestedFileName, object partId, CancellationToken cancellationToken)
        {
            using (IReportPart reportPart = await CreatePartAsync(suggestedFileName, partId, cancellationToken))
                await reportPart.Stream.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
        }

        public async Task<IReportPart> CreatePartAsync(string suggestedFileName, object partId, CancellationToken cancellationToken)
        {
            IDisposable writeLock = await WriteLockAsync(cancellationToken);
            Stream entryStream = _zipArchive.CreateEntry(suggestedFileName).Open();

            return new ReportPart(entryStream, writeLock);
        }

        public Task ReportAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _zipArchive.Dispose();
            _zipArchive = null;

            cancellationToken.ThrowIfCancellationRequested();

            _fileStream.Dispose();
            _fileStream = null;

            return Task.CompletedTask;
        }

        public Task CleanAfterCancelAsync()
        {
            if (_zipArchive != null)
            {
                foreach (ZipArchiveEntry entry in _zipArchive.Entries.ToArray())
                    entry.Delete();

                _zipArchive.Dispose();
            }

            _fileStream?.Dispose();

            if (File.Exists(FilePath))
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