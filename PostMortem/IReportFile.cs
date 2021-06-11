namespace PostMortem
{
    public interface IReportFile : IReportPart
    {
        string FilePath { get; }
    }
}