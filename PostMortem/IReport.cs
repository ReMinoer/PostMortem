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
        Task AddFilePartAsync(string filePath, object partId, bool removeFile, CancellationToken cancellationToken);
        Task AddTextPartAsync(string text, string suggestedFileName, object partId, CancellationToken cancellationToken);
        Task AddBytesPartAsync(byte[] bytes, string suggestedFileName, object partId, CancellationToken cancellationToken);
        Task<IReportPart> CreatePartAsync(string suggestedFileName, object partId, CancellationToken cancellationToken);
        Task ReportAsync(CancellationToken cancellationToken);
        Task CleanAfterCancelAsync();
    }
}