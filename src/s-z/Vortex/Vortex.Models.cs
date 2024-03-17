namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class VortexResult : IResult
{
    public DateTime Timestamp { get; set; }
    public double? Pvi { get; set; }
    public double? Nvi { get; set; }
}
