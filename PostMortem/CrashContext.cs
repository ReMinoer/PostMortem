using System;
using System.Diagnostics;
using System.Threading;

namespace PostMortem
{
    public class CrashContext : ICrashContext
    {
        static public CrashContext FromException(Exception exception, string sourceName = null)
            => BuildCrashContext(exception, sourceName, unhandled: false);
        static public CrashContext FromUnhandledException(Exception unhandledException, string sourceName = null)
            => BuildCrashContext(unhandledException, sourceName, unhandled: true);

        static private CrashContext BuildCrashContext(Exception exception, string sourceName, bool unhandled)
            => new CrashContext(exception, sourceName ?? Process.GetCurrentProcess().ProcessName, DateTime.Now, unhandled);

        public Exception Exception { get; set; }
        public string SourceName { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Unhandled { get; set; }

        private readonly CancellationTokenSource _cancellation;
        public CancellationToken CancellationToken => _cancellation.Token;

        public CrashContext(Exception exception, string sourceName, DateTime timestamp, bool unhandled)
        {
            Exception = exception;
            SourceName = sourceName;
            Timestamp = timestamp;
            Unhandled = unhandled;
            _cancellation = new CancellationTokenSource();
        }

        public void Cancel() => _cancellation.Cancel();
    }
}