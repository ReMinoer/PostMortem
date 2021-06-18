using System;
using System.Threading;

namespace PostMortem
{
    public interface ICrashContext
    {
        Exception Exception { get; }
        string SourceName { get; }
        DateTime Timestamp { get; }
        bool Unhandled { get; }
        CancellationToken CancellationToken { get; }
        void Cancel();
    }
}