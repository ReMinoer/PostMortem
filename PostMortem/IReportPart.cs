namespace PostMortem
{
    public interface IReportPart
    {
        bool CanReport { get; }
        string SuggestedFileName { get; }
    }
}