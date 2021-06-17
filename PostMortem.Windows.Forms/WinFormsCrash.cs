using System.Threading;
using System.Windows.Forms;

namespace PostMortem.Windows.Forms
{
    static public class WinFormsCrash
    {
        static public void SetupUnhandledExceptions(string sourceName, ICrashHandler crashHandler, IReport report, CancellationToken cancellationToken)
        {
            Application.ThreadException += OnUnhandledException;

            void OnUnhandledException(object sender, ThreadExceptionEventArgs e)
            {
                Application.ThreadException -= OnUnhandledException;

                CrashContext crashContext = CrashContext.FromUnhandledException(e.Exception, sourceName);
                Crash.Handle(crashContext, crashHandler, report, cancellationToken);
            }
        }
    }
}