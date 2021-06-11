using System.IO;
using PostMortem.Utils.Base;

namespace PostMortem.Utils
{
    public class CrashMultiPathProvider : CrashPathProviderBase<CrashMultiPathProvider.Provider>
    {
        public delegate string Provider(ICrashContext crashContext, string id);

        public string GetPath(ICrashContext crashContext, string id)
        {
            ThrowIfNoFolderPath();

            string name = GetName(crashContext, id);
            string folderPath = FolderPath ?? FolderPathProvider.Invoke(crashContext, id);

            return Path.Combine(folderPath, name);
        }

        public string GetName(ICrashContext crashContext, string id)
        {
            ThrowIfNoName();

            return Name ?? NameProvider.Invoke(crashContext, id);
        }
    }
}