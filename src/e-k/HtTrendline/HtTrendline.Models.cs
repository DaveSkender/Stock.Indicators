namespace Skender.Stock.Indicators;

public sealed class HtlResult : ResultBase, IReusableResult
{
    public int? DcPeriods { get; set; }
    public double? Trendline { get; set; }
    public double? SmoothPrice { get; set; }

    double IReusableResult.Value => Trendline.Null2NaN();
}
