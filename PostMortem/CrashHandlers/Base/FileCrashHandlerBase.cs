using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PostMortem.CrashHandlers.Base
{
    public abstract class FileCrashHandlerBase : SinglePartCrashHandlerBase
    {
        public string FilePath { get; private set; }
        protected abstract bool RemoveReportedFile { get; }

        public override bool HandleCrashImmediately(ICrashContext crashContext)
        {
            if (!base.HandleCrashImmediately(crashContext))
                return false;

            FilePath = PathProvider.GetPath(crashContext);
            return true;
        }

        protected override sealed async Task CreatePartAsync(ICrashContext crashContext, IReport report, CancellationToken cancellationToken)
        {
            try
            {
                await WriteFileAsync(FilePath, crashContext, cancellationToken);
                await report.AddFilePartAsync(FilePath, PartId, RemoveReportedFile, cancellationToken);
            }
            catch (Exception)
            {
                if (File.Exists(FilePath))
                    await Task.Run(() => File.Delete(FilePath));

                throw;
            }
        }
        
        protected abstract Task WriteFileAsync(string filePath, ICrashContext crashContext, CancellationToken cancellationToken);

        public override Task CleanAfterCancelAsync()
        {
            if (File.Exists(FilePath))
                return Task.Run(() => File.Delete(FilePath));

            return Task.CompletedTask;
        }
    }
}