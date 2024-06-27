namespace Skender.Stock.Indicators;

public record struct HtlResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public int? DcPeriods { get; set; }
    public double? Trendline { get; set; }
    public double? SmoothPrice { get; set; }

    readonly double IReusable.Value
        => Trendline.Null2NaN();
}
