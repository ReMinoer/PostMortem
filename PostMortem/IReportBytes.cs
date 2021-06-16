namespace PostMortem
{
    public interface IReportBytes : IReportPart
    {
        byte[] Bytes { get; }
    }
}