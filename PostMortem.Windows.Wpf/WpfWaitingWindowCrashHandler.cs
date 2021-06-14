using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using PostMortem.CrashHandlers.Base;

namespace PostMortem.Windows.Wpf
{
    public class WpfWaitingWindowCrashHandler : CrashHandlerBase
    {
        private Window _window;

        public string Message { get; set; }
        public string Caption { get; set; }
        public Uri IconUri { get; set; }

        public override Task<bool> HandleCrashAsync(ICrashContext crashContext, CancellationToken cancellationToken)
        {
            var thread = new Thread(ShowWindow);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Priority = ThreadPriority.Highest;
            thread.IsBackground = true;
            thread.Start(crashContext);

            return Task.FromResult(true);
        }

        private void ShowWindow(object args)
        {
            IntPtr mainWindowHandle = Process.GetCurrentProcess().MainWindowHandle;
            ICrashContext crashContext = (CrashContext)args;

            _window = new Window
            {
                Title = Caption ?? crashContext.SourceName,
                Icon = IconUri != null ? new BitmapImage(IconUri) : null,
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Topmost = true,
                Content = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Children =
                    {
                        new Label
                        {
                            Content = Message ?? "Collecting crash data...\nPlease wait. It can take a few seconds...",
                            Margin = new Thickness(10, 5, 10, 10)
                        },
                        new ProgressBar
                        {
                            IsIndeterminate = true,
                            Height = 24,
                            Margin = new Thickness(10, 0, 10, 10)
                        }
                    }
                }
            };
            
            _window.Closing += PreventClosing;
            _window.Show();

            if (mainWindowHandle != IntPtr.Zero)
                NativeMethodHelpers.SetOwnerWindow(new WindowInteropHelper(_window).EnsureHandle(), mainWindowHandle);

            Dispatcher.Run();
        }

        public override Task ConfigureReportAsync(IReport report, CancellationToken cancellationToken)
        {
            report.Reported += OnReported;

            void OnReported(object sender, EventArgs args)
            {
                report.Reported -= OnReported;

                _window.Closing -= PreventClosing;
                
                if (_window.Dispatcher.CheckAccess())
                    _window.Close();
                else
                    _window.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(_window.Close));
            };

            return Task.CompletedTask;
        }

        private void PreventClosing(object sender, CancelEventArgs args)
        {
            args.Cancel = true;
        }
    }
}