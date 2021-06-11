using System;
using System.Threading;
using System.Threading.Tasks;

namespace PostMortem
{
    static public class Crash
    {
        static public async Task HandleAndReportAsync(string sourceName, Exception exception, ICrashHandler crashHandler, IReport report, CancellationToken cancellationToken)
        {
            ICrashContext crashContext = CrashContext.FromException(exception, sourceName);
            if (await crashHandler.HandleCrashAsync(crashContext, cancellationToken))
                await ReportAsync(crashContext, crashHandler, report, cancellationToken);
        }

        static public async Task ReportAsync(ICrashContext crashContext, ICrashHandler crashHandler, IReport report, CancellationToken cancellationToken)
        {
            await report.PrepareAsync(crashContext, cancellationToken);
            await crashHandler.ConfigureReportAsync(report, cancellationToken);
            await report.ProcessAsync(cancellationToken);
        }

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
                if (Task.Run(async () => await crashHandler.HandleCrashAsync(crashContext, cancellationToken), cancellationToken).Result)
                    Task.Run(async () => await ReportAsync(crashContext, crashHandler, report, cancellationToken), cancellationToken).Wait(cancellationToken);
            }
        }
    }
}