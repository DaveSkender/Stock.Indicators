namespace Skender.Stock.Indicators;

[Serializable]
public record SlopeResult
(
    DateTime Timestamp,
    double? Slope = null,
    double? Intercept = null,
    double? StdDev = null,
    double? RSquared = null,
    decimal? Line = null // last line segment only
) : IReusable
{
    public double Value => Slope.Null2NaN();
}
