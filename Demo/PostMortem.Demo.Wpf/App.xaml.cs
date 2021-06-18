using System.Threading;
using System.Windows;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using PostMortem.CrashHandlers;
using PostMortem.Logging;
using PostMortem.Reports;
using PostMortem.Windows;
using PostMortem.Windows.Screenshots;
using PostMortem.Windows.Wpf;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PostMortem.Demo.Wpf
{
    public partial class App : Application
    {
        static public ILoggerProvider LoggerProvider { get; } = new NLogLoggerProvider();

        public App()
        {
            var applicationName = typeof(App).Namespace;
            ILogger logger = LoggerProvider.CreateLogger(applicationName);

            var crashHandler = new SequentialCrashHandlers
            {
                new ConsoleCrashHandler(),
                new LogCrashHandler(logger),
                WpfMessageBoxCrashHandler.AskUser,
                new WpfWaitingWindowCrashHandler(),
                new ParallelCrashHandlers
                {
                    AlwaysContinue = true,
                    CrashHandlers =
                    {
                        new ExceptionInfoCrashHandler(),
                        new WindowsProcessDumpCrashHandler(ProcessDumpType.Full),
                        new ScreenshotCrashHandler(),
                        new FilePatternCrashHandler("*.pdb")
                    }
                }
            };

            var report = new ShowInExplorerReport<ZipArchiveReport>(new ZipArchiveReport(), x => x.FilePath);

            WpfCrash.SetupUnhandledExceptions(nameof(PostMortem), crashHandler, report, CancellationToken.None);
        }
    }
}
