using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PostMortem.CrashHandlers.Base;

namespace PostMortem.Windows.Forms
{
    public class WinFormsWaitingWindowCrashHandler : CrashHandlerBase
    {
        private Form _window;

        public string Message { get; set; }
        public string Caption { get; set; }
        public string IconPath { get; set; }

        public override bool HandleCrashImmediately(ICrashContext crashContext) => true;

        public override Task<bool> HandleCrashAsync(ICrashContext crashContext, IReport report, CancellationToken cancellationToken)
        {
            report.Reported += OnReported;

            void OnReported(object sender, EventArgs args)
            {
                report.Reported -= OnReported;

                _window.Closing -= PreventClosing;
                _window.Close();
            };

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
                            }
                        }
                    }
                }
            };
            
            Application.EnableVisualStyles();

            _window.Closing += PreventClosing;
            _window.Show();

            if (mainWindowHandle != IntPtr.Zero)
                NativeMethodHelpers.SetOwnerWindow(_window.Handle, mainWindowHandle);
            
            Application.Run();
        }

        private void PreventClosing(object sender, CancelEventArgs args)
        {
            args.Cancel = true;
        }
    }
}