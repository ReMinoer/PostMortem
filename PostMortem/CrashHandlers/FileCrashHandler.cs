using System.Threading;
using System.Threading.Tasks;
using PostMortem.CrashHandlers.Base;

namespace PostMortem.CrashHandlers
{
    public class FileCrashHandler : CrashHandlerBase
    {
        public string FilePath { get; set; }
        public bool DeleteFile { get; set; }
        public object PartId { get; set; }

        public FileCrashHandler()
        {
        }

        public FileCrashHandler(string filePath)
        {
            FilePath = filePath;
        }

        public override bool HandleCrashImmediately(ICrashContext crashContext) => true;

        public override async Task<bool> HandleCrashAsync(ICrashContext crashContext, IReport report, CancellationToken cancellationToken)
        {
            await report.AddFilePartAsync(FilePath, PartId, DeleteFile, cancellationToken);
            return true;
        }
    }
}