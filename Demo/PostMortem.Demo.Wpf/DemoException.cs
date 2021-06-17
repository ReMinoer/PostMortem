using System;

namespace PostMortem.Demo.Wpf
{
    public class DemoException : Exception
    {
        public DemoException()
            : base("An exception was triggered by user.")
        {
        }
    }
}