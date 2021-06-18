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

        public override bool HandleCrashImmediately(ICrashContext crashContext) => true;

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

            var readyTaskSource = new TaskCompletionSource<bool>();

            var thread = new Thread(ShowWindow);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Priority = ThreadPriority.Highest;
            thread.IsBackground = true;
            thread.Start((crashContext, readyTaskSource));

            return readyTaskSource.Task;
        }

        public override Task CleanAfterCancelAsync()
        {
            CloseWindow();
            return Task.CompletedTask;
        }

        private void ShowWindow(object args)
        {
            (ICrashContext crashContext, TaskCompletionSource<bool> readyTaskSource) = ((ICrashContext, TaskCompletionSource<bool>))args;
            ShowWindow(crashContext, readyTaskSource);
        }

        protected abstract void ShowWindow(ICrashContext crashContext, TaskCompletionSource<bool> readyTaskSource);
        protected abstract void CloseWindow();
    }
}