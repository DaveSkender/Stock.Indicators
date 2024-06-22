namespace Skender.Stock.Indicators;

public record struct HtlResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public int? DcPeriods { get; set; }
    public double? Trendline { get; set; }
    public double? SmoothPrice { get; set; }

    readonly double IReusableResult.Value
        => Trendline.Null2NaN();
}
