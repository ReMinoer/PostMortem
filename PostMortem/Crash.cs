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
                Handle(crashContext, crashHandler, report, cancellationToken);
            }
        }

        static public void Handle(string sourceName, Exception exception, ICrashHandler crashHandler, IReport report, CancellationToken cancellationToken)
        {
            ICrashContext crashContext = CrashContext.FromException(exception, sourceName);
            Handle(crashContext, crashHandler, report, cancellationToken);
        }

        static public void Handle(ICrashContext crashContext, ICrashHandler crashHandler, IReport report, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, crashContext.CancellationToken).Token;
                cancellationToken.ThrowIfCancellationRequested();

                if (!crashHandler.HandleCrashImmediately(crashContext))
                {
                    CleanAfterCancel(crashHandler, report);
                    return;
                }

                Task.Run(async () =>
                {
                    await report.PrepareAsync(crashContext, cancellationToken);

                    if (!await crashHandler.HandleCrashAsync(crashContext, report, cancellationToken))
                    {
                        CleanAfterCancel(crashHandler, report);
                        return;
                    }

                    await report.ReportAsync(cancellationToken);

                }, cancellationToken).Wait(CancellationToken.None);
            }
            catch (AggregateException e)
            {
                foreach (Exception innerException in e.InnerExceptions)
                {
                    if (innerException is OperationCanceledException)
                        CleanAfterCancel(crashHandler, report);
                    else
                        throw innerException;
                }
            }
            catch (Exception)
            {
                CleanAfterCancel(crashHandler, report);
            }
        }

        static private void CleanAfterCancel(ICrashHandler crashHandler, IReport report)
        {
            Task.Run(async () =>
            {
                await crashHandler.CleanAfterCancelAsync();
                await report.CleanAfterCancelAsync();

            }).Wait();
        }
    }
}