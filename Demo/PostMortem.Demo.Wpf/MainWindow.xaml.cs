using System;
using System.Threading;
using System.Windows;
using Microsoft.Extensions.Logging;
using PostMortem.CrashHandlers;
using PostMortem.Logging;
using PostMortem.Reports;
using PostMortem.Windows;
using PostMortem.Windows.Screenshots;
using PostMortem.Windows.Wpf;

namespace PostMortem.Demo.Wpf
{
    public partial class MainWindow : Window
    {
        private readonly ICrashHandler _crashHandler;
        private readonly IReport _report;

        public MainWindow()
        {
            ILogger logger = App.LoggerProvider.CreateLogger(nameof(MainWindow));

            _crashHandler = new SequentialCrashHandlers
            {
                new SequentialCrashHandlers
                {
                    new ConsoleCrashHandler(),
                    new LogCrashHandler(logger),
                    MessageBoxCrashHandler.InformUser
                },
                new ParallelCrashHandlers
                {
                    new ExceptionInfoFileCrashHandler(),
                    new ScreenshotCrashHandler()
                }
            };

            var zipArchiveReport = new ZipArchiveReport();
            _report = new ShowInExplorerReport(zipArchiveReport, zipArchiveReport);

            InitializeComponent();
        }

        private async void OnTriggerHandledException(object sender, RoutedEventArgs e)
        {
            try
            {
                throw new DemoException("An exception was triggered by user on main thread.");
            }
            catch (Exception exception)
            {
                await Crash.HandleAndReportAsync(nameof(PostMortem), exception, _crashHandler, _report, CancellationToken.None);
            }
        }

        private void OnTriggerUnhandledException(object sender, RoutedEventArgs e)
        {
            throw new DemoException("An unhandled exception was triggered by user on main thread.");
        }
    }
}
