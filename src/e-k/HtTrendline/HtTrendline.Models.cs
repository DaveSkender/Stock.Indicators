namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class HtlResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public int? DcPeriods { get; set; }
    public double? Trendline { get; set; }
    public double? SmoothPrice { get; set; }

    double IReusableResult.Value => Trendline.Null2NaN();
}
