namespace Skender.Stock.Indicators;

[Serializable]
public record HtlResult
(
    DateTime Timestamp,
    int? DcPeriods,
    double? Trendline,
    double? SmoothPrice
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Trendline.Null2NaN();
}
