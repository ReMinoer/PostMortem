using System;

namespace PostMortem.Demo.Wpf
{
    public class DemoException : Exception
    {
        public DemoException(string message)
            : base(message)
        {
        }
    }
}