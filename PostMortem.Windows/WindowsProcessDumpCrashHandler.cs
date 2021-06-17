using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PostMortem.CrashHandlers.Base;
using PostMortem.Utils;
using PostMortem.Windows.Dumper;

namespace PostMortem.Windows
{
    public class WindowsProcessDumpCrashHandler : FileCrashHandlerBase
    {
        public string ProcessName { get; set; }
        public ProcessDumpType DumpType { get; set; } = ProcessDumpType.Mini;

        protected override bool RemoveFile => true;

        public WindowsProcessDumpCrashHandler()
        {
        }

        public WindowsProcessDumpCrashHandler(ProcessDumpType dumpType)
        {
            DumpType = dumpType;
        }

        public WindowsProcessDumpCrashHandler(string processName, ProcessDumpType dumpType)
        {
            ProcessName = processName;
            DumpType = dumpType;
        }

        protected override string GetDefaultFileName(ICrashContext crashContext) => crashContext.GetDefaultFileName("process_dump", "dmp");

        public override bool HandleCrashImmediately(ICrashContext crashContext)
        {
            if (!base.HandleCrashImmediately(crashContext))
                return false;

            Process process = ProcessName != null
                ? Process.GetProcessesByName(ProcessName).FirstOrDefault()
                : Process.GetCurrentProcess();

            if (process == null)
                throw new ProcessDumpingException($"Process \"{ProcessName}\" not found.");

            WindowsProcessDumper.Dump(process, FilePath, DumpType);
            return true;
        }

        protected override Task WriteFileAsync(string filePath, ICrashContext crashContext, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}