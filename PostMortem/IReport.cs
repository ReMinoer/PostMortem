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
        Task AddFileAsync(IReportFile reportFile, object partId, bool removeFile, CancellationToken cancellationToken);
        Task AddTextAsync(IReportText reportText, object partId, CancellationToken cancellationToken);
        Task AddBytesAsync(IReportBytes reportBytes, object partId, CancellationToken cancellationToken);
        Task<IPartStream> CreatePartStreamAsync(IReportPart reportPart, object partId, CancellationToken cancellationToken);
        Task ReportAsync(CancellationToken cancellationToken);
        Task CancelAsync();
    }
}