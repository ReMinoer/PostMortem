using System;
using System.Threading;
using System.Threading.Tasks;

namespace PostMortem
{
    static public class Crash
    {
        static public void SetupUnhandledExceptions(string sourceName, ICrashHandler crashHandler, IReport report, CancellationToken cancellationToken)
        {
            SetupUnhandledExceptions(AppDomain.CurrentDomain, sourceName, crashHandler, report, cancellationToken);
        }

        static public void SetupUnhandledExceptions(AppDomain appDomain, string sourceName, ICrashHandler crashHandler, IReport report, CancellationToken cancellationToken)
        {
            appDomain.UnhandledException += OnUnhandledException;

            void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
            {
                appDomain.UnhandledException -= OnUnhandledException;

                ICrashContext crashContext = CrashContext.FromUnhandledException((Exception)e.ExceptionObject, sourceName);
                Task.Run(async () => await HandleAsync(crashContext, crashHandler, report, cancellationToken), cancellationToken).Wait(CancellationToken.None);
            }
        }
        
        static public async Task HandleAsync(string sourceName, Exception exception, ICrashHandler crashHandler, IReport report, CancellationToken cancellationToken)
        {
            ICrashContext crashContext = CrashContext.FromException(exception, sourceName);
            await HandleAsync(crashContext, crashHandler, report, cancellationToken);
        }

        static public async Task HandleAsync(ICrashContext crashContext, ICrashHandler crashHandler, IReport report, CancellationToken cancellationToken)
        {
            try
            {
                await report.PrepareAsync(crashContext, cancellationToken);

                if (!await crashHandler.HandleCrashAsync(crashContext, report, cancellationToken))
                {
                    await report.CancelAsync();
                    return;
                }

                await report.ReportAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                await report.CancelAsync();
            }
        }
    }
}