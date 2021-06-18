using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using PostMortem.Windows.Base;
using PostMortem.Windows.Utils;

namespace PostMortem.Windows.Forms
{
    public class WinFormsWaitingWindowCrashHandler : WaitingWindowCrashHandlerBase
    {
        private Form _window;
        public string IconPath { get; set; }

        protected override void ShowWindow(ICrashContext crashContext, EventWaitHandle readyEvent)
        {
            IntPtr mainWindowHandle = Process.GetCurrentProcess().MainWindowHandle;

            Button cancelButton;
            _window = new Form
            {
                Text = Caption ?? crashContext.SourceName,
                Icon = IconPath != null ? new Icon(IconPath) : null,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FormBorderStyle = FormBorderStyle.FixedSingle,
                MinimizeBox = false,
                MaximizeBox = false,
                StartPosition = FormStartPosition.CenterScreen,
                TopMost = true,
                Controls =
                {
                    new FlowLayoutPanel
                    {
                        FlowDirection = FlowDirection.TopDown,
                        AutoSize = true,
                        AutoSizeMode = AutoSizeMode.GrowAndShrink,
                        Controls =
                        {
                            new Label
                            {
                                Text = Message ?? "Collecting crash data...\nPlease wait. It can take a few seconds...",
                                Margin = new Padding(10, 10, 10, 10),
                                AutoSize = true
                            },
                            new ProgressBar
                            {
                                Style = ProgressBarStyle.Marquee,
                                Margin = new Padding(10, 0, 10, 10),
                                Dock = DockStyle.Bottom,
                                AutoSize = true
                            },
                            (cancelButton = new Button
                            {
                                Text = CancelButtonText ?? "Cancel",
                                Visible = IsCancellable,
                                Margin = new Padding(10, 0, 10, 10),
                                Dock = DockStyle.Right
                            })
                        }
                    }
                }
            };

            cancelButton.Click += (s, e) =>
            {
                cancelButton.Enabled = false;
                crashContext.Cancel();
            };

            Application.EnableVisualStyles();

            _window.Closing += PreventClosing;
            _window.Show();

            if (mainWindowHandle != IntPtr.Zero)
                NativeMethodHelpers.SetOwnerWindow(_window.Handle, mainWindowHandle);

            readyEvent.Set();
            Application.Run();
        }

        protected override void CloseWindow()
        {
            if (_window == null)
                return;

            _window.Closing -= PreventClosing;
            _window.Close();
            _window.Dispose();
            _window = null;
        }

        private void PreventClosing(object sender, CancelEventArgs args)
        {
            args.Cancel = true;
        }
    }
}