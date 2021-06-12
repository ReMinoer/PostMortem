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
                new SequentialCrashHandlers
                {
                    new ConsoleCrashHandler(),
                    new LogCrashHandler(logger),
                    MessageBoxCrashHandler.AskUser
                },
                new ParallelCrashHandlers
                {
                    AlwaysContinue = true,
                    CrashHandlers =
                    {
                        new ExceptionInfoFileCrashHandler(),
                        new WindowsProcessDumpCrashHandler(ProcessDumpType.Full),
                        new ScreenshotCrashHandler()
                    }
                }
            };

            var zipArchiveReport = new ZipArchiveReport();
            var report = new ShowInExplorerReport(zipArchiveReport, zipArchiveReport);

            Crash.SetupHandleAndReportOnUnhandledException(nameof(PostMortem), crashHandler, report, CancellationToken.None);
        }
    }
}
