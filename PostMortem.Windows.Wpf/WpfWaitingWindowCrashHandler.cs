using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using PostMortem.Windows.Base;
using PostMortem.Windows.Utils;

namespace PostMortem.Windows.Wpf
{
    public class WpfWaitingWindowCrashHandler : WaitingWindowCrashHandlerBase
    {
        private Window _window;
        public Uri IconUri { get; set; }

        protected override void ShowWindow(ICrashContext crashContext, EventWaitHandle readyEvent)
        {
            IntPtr mainWindowHandle = Process.GetCurrentProcess().MainWindowHandle;

            Button cancelButton;
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
                        },
                        (cancelButton = new Button
                        {
                            Content = CancelButtonText ?? "Cancel",
                            Visibility = IsCancellable ? Visibility.Visible : Visibility.Collapsed,
                            Height = 24,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            Margin = new Thickness(10, 0, 10, 10),
                            Padding = new Thickness(10, 0, 10, 0)
                        })
                    }
                }
            };

            Grid.SetColumn(cancelButton, 1);

            cancelButton.Click += (s, e) =>
            {
                cancelButton.IsEnabled = false;
                crashContext.Cancel();
            };

            _window.Closing += PreventClosing;
            _window.Show();

            if (mainWindowHandle != IntPtr.Zero)
                NativeMethodHelpers.SetOwnerWindow(new WindowInteropHelper(_window).EnsureHandle(), mainWindowHandle);

            readyEvent.Set();
            Dispatcher.Run();
        }

        protected override void CloseWindow()
        {
            if (_window == null)
                return;

            _window.Closing -= PreventClosing;

            if (_window.Dispatcher.CheckAccess())
                _window.Close();
            else
                _window.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(_window.Close));
        }

        private void PreventClosing(object sender, CancelEventArgs args)
        {
            args.Cancel = true;
        }
    }
}