using System.Threading;
using System.Threading.Tasks;

namespace PostMortem
{
    public interface ICrashHandler
    {
        bool HandleCrashImmediately(ICrashContext crashContext);
        Task<bool> HandleCrashAsync(ICrashContext crashContext, IReport report, CancellationToken cancellationToken);
    }
}