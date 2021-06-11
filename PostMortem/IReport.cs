using System.Threading;
using System.Threading.Tasks;

namespace PostMortem
{
    public interface IReport
    {
        Task PrepareAsync(ICrashContext crashContext, CancellationToken cancellationToken);
        Task AddFileAsync(IReportFile reportFile, CancellationToken cancellationToken);
        Task AddTextAsync(IReportText reportText, CancellationToken cancellationToken);
        Task AddBytesAsync(IReportBytes reportBytes, CancellationToken cancellationToken);
        Task ProcessAsync(CancellationToken cancellationToken);
    }
}