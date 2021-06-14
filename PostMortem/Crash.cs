using System;
using System.Threading;
using System.Threading.Tasks;

namespace PostMortem
{
    static public class Crash
    {
        static public void SetupHandleAndReportOnUnhandledException(string sourceName, ICrashHandler crashHandler, IReport report, CancellationToken cancellationToken)
        {
            SetupHandleAndReportOnUnhandledException(AppDomain.CurrentDomain, sourceName, crashHandler, report, cancellationToken);
        }

        static public void SetupHandleAndReportOnUnhandledException(AppDomain appDomain, string sourceName, ICrashHandler crashHandler, IReport report, CancellationToken cancellationToken)
        {
            appDomain.UnhandledException += OnUnhandledException;

            void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
            {
                appDomain.UnhandledException -= OnUnhandledException;

                ICrashContext crashContext = CrashContext.FromUnhandledException((Exception)e.ExceptionObject, sourceName);
                Task.Run(async () => await HandleAndReportAsync(crashContext, crashHandler, report, cancellationToken), cancellationToken).Wait(CancellationToken.None);
            }
        }
        
        static public async Task HandleAndReportAsync(string sourceName, Exception exception, ICrashHandler crashHandler, IReport report, CancellationToken cancellationToken)
        {
            ICrashContext crashContext = CrashContext.FromException(exception, sourceName);
            await HandleAndReportAsync(crashContext, crashHandler, report, cancellationToken);
        }

        static public async Task HandleAndReportAsync(ICrashContext crashContext, ICrashHandler crashHandler, IReport report, CancellationToken cancellationToken)
        {
            try
            {
                if (!await crashHandler.HandleCrashAsync(crashContext, cancellationToken))
                    return;

                await report.PrepareAsync(crashContext, cancellationToken);
                await crashHandler.ConfigureReportAsync(report, cancellationToken);
                await report.ReportAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                await report.CancelAsync();
            }
        }
    }
}