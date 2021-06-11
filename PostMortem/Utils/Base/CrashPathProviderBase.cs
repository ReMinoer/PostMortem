using System;
using System.IO;

namespace PostMortem.Utils.Base
{
    public class CrashPathProviderBase<TProvider>
    {
        public string Name { get; set; }
        public TProvider NameProvider { get; set; }
        public bool HasName => Name != null || NameProvider != null;

        public string FolderPath { get; set; } = Path.GetTempPath();
        public TProvider FolderPathProvider { get; set; }
        public bool HasFolderPath => FolderPath != null || FolderPathProvider != null;

        protected void ThrowIfNoName()
        {
            if (!HasName)
                throw new InvalidOperationException("No name provided to build complete path.");
        }

        protected void ThrowIfNoFolderPath()
        {
            if (!HasFolderPath)
                throw new InvalidOperationException("No folder path provided to build complete path.");
        }
    }
}