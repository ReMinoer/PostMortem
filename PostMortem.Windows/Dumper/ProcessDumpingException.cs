using System;

namespace PostMortem.Windows.Dumper
{
    public class ProcessDumpingException : Exception
    {
        public ProcessDumpingException(string message)
            : base(message)
        {
        }

        public ProcessDumpingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}