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

        protected override Task WriteFileAsync(string filePath, ICrashContext crashContext, CancellationToken cancellationToken)
        {
            Process process = ProcessName != null
                ? Process.GetProcessesByName(ProcessName).FirstOrDefault()
                : Process.GetCurrentProcess();

            if (process == null)
                throw new ProcessDumpingException($"Process \"{ProcessName}\" not found.");
            
            return Task.Run(() => WindowsProcessDumper.Dump(process, filePath, DumpType), cancellationToken);
        }
    }
}