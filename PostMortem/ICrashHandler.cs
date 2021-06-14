using System.Threading;
using System.Threading.Tasks;

namespace PostMortem
{
    public interface ICrashHandler
    {
        Task<bool> HandleCrashAsync(ICrashContext crashContext, CancellationToken cancellationToken);
        Task ConfigureReportAsync(IReport report, CancellationToken cancellationToken);
    }
}