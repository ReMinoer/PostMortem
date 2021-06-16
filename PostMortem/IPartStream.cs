using System;
using System.IO;

namespace PostMortem
{
    public interface IPartStream : IDisposable
    {
        Stream Stream { get; }
    }
}