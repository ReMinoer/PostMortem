namespace PostMortem
{
    public interface IReportBytes : IReportPart
    {
        byte[] Bytes { get; }
        string SuggestedFileName { get; }
    }
}