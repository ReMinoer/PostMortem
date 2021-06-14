using System;

namespace PostMortem
{
    public interface ICrashContext
    {
        Exception Exception { get; }
        string SourceName { get; }
        DateTime Timestamp { get; }
        bool Unhandled { get; }
    }
}