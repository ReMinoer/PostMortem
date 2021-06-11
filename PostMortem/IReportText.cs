namespace PostMortem
{
    public interface IReportText: IReportPart
    {
        string Text { get; }
        string SuggestedFileName { get; }
    }
}