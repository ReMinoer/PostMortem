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
        public virtual Task AddFileAsync(IReportFile reportFile, object partId, bool removeFile, CancellationToken cancellationToken) => BaseReport?.AddFileAsync(reportFile, partId, removeFile, cancellationToken) ?? Task.CompletedTask;
        public virtual Task AddTextAsync(IReportText reportText, object partId, CancellationToken cancellationToken) => BaseReport?.AddTextAsync(reportText, partId, cancellationToken) ?? Task.CompletedTask;
        public virtual Task AddBytesAsync(IReportBytes reportBytes, object partId, CancellationToken cancellationToken) => BaseReport?.AddBytesAsync(reportBytes, partId, cancellationToken) ?? Task.CompletedTask;
        public virtual Task<IPartStream> CreatePartStreamAsync(IReportPart reportPart, object partId, CancellationToken cancellationToken) => BaseReport?.CreatePartStreamAsync(reportPart, partId, cancellationToken) ?? Task.FromResult<IPartStream>(null);
        public virtual Task ReportAsync(CancellationToken cancellationToken) => BaseReport?.ReportAsync(cancellationToken) ?? Task.CompletedTask;
        public virtual Task CancelAsync() => BaseReport?.CancelAsync() ?? Task.CompletedTask;
    }
}