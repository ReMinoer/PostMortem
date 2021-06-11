using System;

namespace PostMortem.Windows.MiniDump
{
    public class MiniDumpGenerationException : Exception
    {
        public MiniDumpGenerationException(string message)
            : base(message)
        {
        }
    }
}