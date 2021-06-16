using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PostMortem.Windows.Wpf
{
    static public class WpfCrash
    {
        static public void SetupUnhandledExceptions(string sourceName, ICrashHandler crashHandler, IReport report, CancellationToken cancellationToken)
        {
            SetupUnhandledExceptions(Application.Current, sourceName, crashHandler, report, cancellationToken);
        }

        static public void SetupUnhandledExceptions(Application application, string sourceName, ICrashHandler crashHandler, IReport report, CancellationToken cancellationToken)
        {
            application.DispatcherUnhandledException += OnUnhandledException;

            void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
            {
                application.DispatcherUnhandledException -= OnUnhandledException;

                CrashContext crashContext = CrashContext.FromUnhandledException(e.Exception, sourceName);
                Task.Run(async () => await Crash.HandleAsync(crashContext, crashHandler, report, cancellationToken), cancellationToken).Wait(CancellationToken.None);
            }
        }

        static public void SetupUnhandledExceptions(Dispatcher dispatcher, string sourceName, ICrashHandler crashHandler, IReport report, CancellationToken cancellationToken)
        {
            dispatcher.UnhandledException += OnUnhandledException;

            void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
            {
                dispatcher.UnhandledException -= OnUnhandledException;

                CrashContext crashContext = CrashContext.FromUnhandledException(e.Exception, sourceName);
                Task.Run(async () => await Crash.HandleAsync(crashContext, crashHandler, report, cancellationToken), cancellationToken).Wait(CancellationToken.None);
            }
        }
    }
}