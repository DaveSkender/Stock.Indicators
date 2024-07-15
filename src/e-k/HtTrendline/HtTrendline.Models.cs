namespace Skender.Stock.Indicators;

public record HtlResult
(
    DateTime Timestamp,
    int? DcPeriods,
    double? Trendline,
    double? SmoothPrice
) : Reusable(Timestamp)
{
    public override double Value => Trendline.Null2NaN();
}
