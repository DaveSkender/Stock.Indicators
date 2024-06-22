namespace Skender.Stock.Indicators;

public record struct StarcBandsResult : IResult
{
    public DateTime Timestamp { get; set; }
    public double? UpperBand { get; set; }
    public double? Centerline { get; set; }
    public double? LowerBand { get; set; }
}
