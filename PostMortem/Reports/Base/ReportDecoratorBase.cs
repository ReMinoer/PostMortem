using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PostMortem.Reports.Base
{
    public abstract class ReportDecoratorBase : IReport
    {
        protected abstract IReport BaseReport { get; }

        public abstract event EventHandler Reported;
        public abstract event EventHandler Cancelled;

        public virtual Task PrepareAsync(ICrashContext crashContext, CancellationToken cancellationToken) => BaseReport?.PrepareAsync(crashContext, cancellationToken) ?? Task.CompletedTask;
        public virtual Task AddFilePartAsync(string filePath, object partId, bool removeFile, CancellationToken cancellationToken) => BaseReport?.AddFilePartAsync(filePath, partId, removeFile, cancellationToken) ?? Task.CompletedTask;
        public virtual Task AddTextPartAsync(string text, string suggestedFileName, object partId, CancellationToken cancellationToken) => BaseReport?.AddTextPartAsync(text, suggestedFileName, partId, cancellationToken) ?? Task.CompletedTask;
        public virtual Task AddBytesPartAsync(byte[] bytes, string suggestedFileName, object partId, CancellationToken cancellationToken) => BaseReport?.AddBytesPartAsync(bytes, suggestedFileName, partId, cancellationToken) ?? Task.CompletedTask;
        public virtual Task<IReportPart> CreatePartAsync(string suggestedFileName, object partId, CancellationToken cancellationToken) => BaseReport?.CreatePartAsync(suggestedFileName, partId, cancellationToken) ?? Task.FromResult<IReportPart>(null);
        public virtual Task ReportAsync(CancellationToken cancellationToken) => BaseReport?.ReportAsync(cancellationToken) ?? Task.CompletedTask;
        public virtual Task CancelAsync() => BaseReport?.CancelAsync() ?? Task.CompletedTask;
    }
}