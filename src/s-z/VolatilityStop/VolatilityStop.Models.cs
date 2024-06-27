namespace Skender.Stock.Indicators;

public record struct VolatilityStopResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Sar { get; set; }
    public bool? IsStop { get; set; }

    // SAR values as long/short stop bands
    public double? UpperBand { get; set; }
    public double? LowerBand { get; set; }

    readonly double IReusable.Value
        => Sar.Null2NaN();
}
