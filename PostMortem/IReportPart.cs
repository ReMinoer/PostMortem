using System;
using System.IO;

namespace PostMortem
{
    public interface IReportPart : IDisposable
    {
        Stream Stream { get; }
    }
}