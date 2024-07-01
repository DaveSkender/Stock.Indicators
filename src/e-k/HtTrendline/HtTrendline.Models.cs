namespace Skender.Stock.Indicators;

public readonly record struct HtlResult
(
    DateTime Timestamp,
    int? DcPeriods,
    double? Trendline,
    double? SmoothPrice
) : IReusable
{
    double IReusable.Value => Trendline.Null2NaN();
}
