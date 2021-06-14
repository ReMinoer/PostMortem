using System;
using System.Diagnostics;

namespace PostMortem
{
    public class CrashContext : ICrashContext
    {
        static public CrashContext FromException(Exception exception, string sourceName = null)
            => new CrashContext(exception, sourceName ?? Process.GetCurrentProcess().ProcessName, DateTime.Now, false);
        static public CrashContext FromUnhandledException(Exception unhandledException, string sourceName = null)
            => new CrashContext(unhandledException, sourceName ?? Process.GetCurrentProcess().ProcessName, DateTime.Now, true);

        public Exception Exception { get; set; }
        public string SourceName { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Unhandled { get; set; }

        public CrashContext(Exception exception, string sourceName, DateTime timestamp, bool unhandled)
        {
            Exception = exception;
            SourceName = sourceName;
            Timestamp = timestamp;
            Unhandled = unhandled;
        }
    }
}