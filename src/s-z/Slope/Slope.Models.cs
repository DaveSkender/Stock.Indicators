namespace Skender.Stock.Indicators;

public record struct SlopeResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Slope { get; set; }
    public double? Intercept { get; set; }
    public double? StdDev { get; set; }
    public double? RSquared { get; set; }
    public decimal? Line { get; set; } // last line segment only

    readonly double IReusableResult.Value
        => Slope.Null2NaN();
}
