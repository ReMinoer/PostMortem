using System.IO;
using PostMortem.Utils.Base;

namespace PostMortem.Utils
{
    public class CrashPathProvider : CrashPathProviderBase<CrashPathProvider.Provider>
    {
        public delegate string Provider(ICrashContext crashContext);

        public string GetPath(ICrashContext crashContext)
        {
            ThrowIfNoFolderPath();

            string name = GetName(crashContext);
            string folderPath = FolderPath ?? FolderPathProvider.Invoke(crashContext);

            return Path.Combine(folderPath, name);
        }

        public string GetName(ICrashContext crashContext)
        {
            ThrowIfNoName();

            return Name ?? NameProvider.Invoke(crashContext);
        }
    }
}