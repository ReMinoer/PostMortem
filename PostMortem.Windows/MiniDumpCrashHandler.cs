using System.Threading;
using System.Threading.Tasks;
using PostMortem.CrashHandlers.Base;
using PostMortem.Utils;
using PostMortem.Windows.MiniDump;

namespace PostMortem.Windows
{
    public class MiniDumpCrashHandler : FileCrashHandlerBase
    {
        protected override string GetDefaultFileName(ICrashContext crashContext) => crashContext.GetDefaultFileName("minidump", "dmp");

        protected override Task WriteFileAsync(string filePath, ICrashContext crashContext, CancellationToken cancellationToken)
        {
            if (crashContext.Unhandled)
                return Task.Run(() => MiniDumpGenerator.SaveUnhandledExceptionMiniDump(filePath, MiniDumpTypes.Normal), cancellationToken);
            return Task.CompletedTask;
        }
    }
}