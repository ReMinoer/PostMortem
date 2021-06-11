using System.Threading;
using System.Threading.Tasks;

namespace PostMortem.Reports.Base
{
    public abstract class ReportDecoratorBase : IReport
    {
        protected abstract IReport BaseReport { get; }

        public virtual Task PrepareAsync(ICrashContext crashContext, CancellationToken cancellationToken) => BaseReport?.PrepareAsync(crashContext, cancellationToken) ?? Task.CompletedTask;
        public virtual Task AddFileAsync(IReportFile reportFile, CancellationToken cancellationToken) => BaseReport?.AddFileAsync(reportFile, cancellationToken) ?? Task.CompletedTask;
        public virtual Task AddTextAsync(IReportText reportText, CancellationToken cancellationToken) => BaseReport?.AddTextAsync(reportText, cancellationToken) ?? Task.CompletedTask;
        public virtual Task AddBytesAsync(IReportBytes reportBytes, CancellationToken cancellationToken) => BaseReport?.AddBytesAsync(reportBytes, cancellationToken) ?? Task.CompletedTask;
        public virtual Task ProcessAsync(CancellationToken cancellationToken) => BaseReport?.ProcessAsync(cancellationToken) ?? Task.CompletedTask;
    }
}