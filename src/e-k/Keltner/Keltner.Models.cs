namespace Skender.Stock.Indicators;

public record struct KeltnerResult : IResult
{
    public DateTime Timestamp { get; set; }
    public double? UpperBand { get; set; }
    public double? Centerline { get; set; }
    public double? LowerBand { get; set; }
    public double? Width { get; set; }
}
