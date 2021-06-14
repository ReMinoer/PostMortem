using System;
using System.Threading;
using System.Threading.Tasks;

namespace PostMortem
{
    public interface IReport
    {
        event EventHandler Reported;
        event EventHandler Cancelled;
        Task PrepareAsync(ICrashContext crashContext, CancellationToken cancellationToken);
        Task AddFileAsync(IReportFile reportFile, bool removeFile, CancellationToken cancellationToken);
        Task AddTextAsync(IReportText reportText, CancellationToken cancellationToken);
        Task AddBytesAsync(IReportBytes reportBytes, CancellationToken cancellationToken);
        Task ReportAsync(CancellationToken cancellationToken);
        Task CancelAsync();
    }
}