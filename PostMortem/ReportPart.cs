using System;
using System.IO;

namespace PostMortem
{
    public class ReportPart : IReportPart
    {
        public Stream Stream { get; }
        private readonly IDisposable[] _disposables;

        public ReportPart(Stream stream, params IDisposable[] disposables)
        {
            Stream = stream;
            _disposables = disposables;
        }

        public void Dispose()
        {
            Stream.Dispose();

            foreach (IDisposable disposable in _disposables)
                disposable.Dispose();
        }
    }
}