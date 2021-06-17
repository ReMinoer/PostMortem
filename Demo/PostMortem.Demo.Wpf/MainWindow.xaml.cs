using System;
using System.Threading;
using System.Threading.Tasks;
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
                new ConsoleCrashHandler(),
                new LogCrashHandler(logger),
                WpfMessageBoxCrashHandler.InformUser,
                new WpfWaitingWindowCrashHandler(),
                new ParallelCrashHandlers
                {
                    new ExceptionInfoCrashHandler(),
                    new WindowsProcessDumpCrashHandler(ProcessDumpType.Full),
                    new ScreenshotCrashHandler(),
                    new FilePatternCrashHandler("*.pdb")
                }
            };
            
            _report = new ShowInExplorerReport<FolderReport>(new FolderReport(), x => x.FolderPath);

            InitializeComponent();
        }

        private void OnTriggerHandledException(object sender, RoutedEventArgs e)
        {
            try
            {
                throw new DemoException();
            }
            catch (Exception exception)
            {
                Crash.Handle(nameof(PostMortem), exception, _crashHandler, _report, CancellationToken.None);
            }
        }

        private void OnTriggerHandledExceptionOnAnotherThread(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    throw new DemoException();
                }
                catch (Exception exception)
                {
                    Crash.Handle(nameof(PostMortem), exception, _crashHandler, _report, CancellationToken.None);
                }
            }).Start();
        }

        private async void OnTriggerHandledExceptionFromTask(object sender, RoutedEventArgs e)
        {
            await Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(100);
                    throw new DemoException();
                }
                catch (Exception exception)
                {
                    Crash.Handle(nameof(PostMortem), exception, _crashHandler, _report, CancellationToken.None);
                }
            });
        }

        private void OnTriggerUnhandledException(object sender, RoutedEventArgs e)
        {
            throw new DemoException();
        }

        private void OnTriggerUnhandledExceptionOnAnotherThread(object sender, RoutedEventArgs e)
        {
            new Thread(() => throw new DemoException()).Start();
        }

        private async void OnTriggerUnhandledExceptionFromTask(object sender, RoutedEventArgs e)
        {
            await Task.Run(async () =>
            {
                await Task.Delay(100);
                throw new DemoException();
            });
        }
    }
}
