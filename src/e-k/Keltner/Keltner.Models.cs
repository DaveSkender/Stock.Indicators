namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class KeltnerResult : IResult
{
    public DateTime Timestamp { get; set; }
    public double? UpperBand { get; set; }
    public double? Centerline { get; set; }
    public double? LowerBand { get; set; }
    public double? Width { get; set; }
}
