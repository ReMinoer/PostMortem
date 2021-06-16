using System.Threading;
using System.Threading.Tasks;

namespace PostMortem
{
    public interface ICrashHandler
    {
        Task<bool> HandleCrashAsync(ICrashContext crashContext, IReport report, CancellationToken cancellationToken);
    }
}