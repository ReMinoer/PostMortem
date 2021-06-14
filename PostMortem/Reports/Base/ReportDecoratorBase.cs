using System;
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
        public virtual Task AddFileAsync(IReportFile reportFile, bool removeFile, CancellationToken cancellationToken) => BaseReport?.AddFileAsync(reportFile, removeFile, cancellationToken) ?? Task.CompletedTask;
        public virtual Task AddTextAsync(IReportText reportText, CancellationToken cancellationToken) => BaseReport?.AddTextAsync(reportText, cancellationToken) ?? Task.CompletedTask;
        public virtual Task AddBytesAsync(IReportBytes reportBytes, CancellationToken cancellationToken) => BaseReport?.AddBytesAsync(reportBytes, cancellationToken) ?? Task.CompletedTask;
        public virtual Task ReportAsync(CancellationToken cancellationToken) => BaseReport?.ReportAsync(cancellationToken) ?? Task.CompletedTask;
        public virtual Task CancelAsync() => BaseReport?.CancelAsync() ?? Task.CompletedTask;
    }
}