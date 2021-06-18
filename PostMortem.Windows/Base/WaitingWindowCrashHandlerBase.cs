using System;
using System.Threading;
using System.Threading.Tasks;
using PostMortem.CrashHandlers.Base;

namespace PostMortem.Windows.Base
{
    public abstract class WaitingWindowCrashHandlerBase : CrashHandlerBase
    {
        public string Caption { get; set; }
        public string Message { get; set; }
        public bool IsCancellable { get; set; } = true;
        public string CancelButtonText { get; set; }

        public override bool HandleCrashImmediately(ICrashContext crashContext)
        {
            using (EventWaitHandle readyEvent = new ManualResetEvent(false))
            {
                var thread = new Thread(ShowWindow);
                thread.SetApartmentState(ApartmentState.STA);
                thread.IsBackground = true;
                thread.Start((crashContext, readyEvent));

                readyEvent.WaitOne();
            }

            return true;
        }

        public override Task<bool> HandleCrashAsync(ICrashContext crashContext, IReport report, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            report.Reported += OnReportedOrCancelled;
            report.Cancelled += OnReportedOrCancelled;

            void OnReportedOrCancelled(object sender, EventArgs args)
            {
                report.Cancelled -= OnReportedOrCancelled;
                report.Reported -= OnReportedOrCancelled;
                CloseWindow();
            }

            return Task.FromResult(true);
        }

        public override Task CleanAfterCancelAsync()
        {
            CloseWindow();
            return Task.CompletedTask;
        }

        private void ShowWindow(object args)
        {
            (ICrashContext crashContext, EventWaitHandle readyEvent) = ((ICrashContext, EventWaitHandle))args;
            ShowWindow(crashContext, readyEvent);
        }

        protected abstract void ShowWindow(ICrashContext crashContext, EventWaitHandle readyEvent);
        protected abstract void CloseWindow();
    }
}