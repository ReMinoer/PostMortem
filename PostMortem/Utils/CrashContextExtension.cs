namespace PostMortem.Utils
{
    static public class CrashContextExtension
    {
        static public string GetDefaultFileName(this ICrashContext crashContext, string prefix, string extension, string id = null)
        {
            return $"{prefix}{id}_{crashContext.SourceName}_{crashContext.Timestamp:yyyy-MM-dd_HH-mm-ss}.{extension}";
        }

        static public string GetDefaultFolderName(this ICrashContext crashContext, string prefix)
        {
            return $"{prefix}_{crashContext.SourceName}_{crashContext.Timestamp:yyyy-MM-dd_HH-mm-ss}";
        }
    }
}