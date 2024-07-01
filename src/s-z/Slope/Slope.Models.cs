namespace Skender.Stock.Indicators;

public readonly record struct SlopeResult
(
    DateTime Timestamp,
    double? Slope,
    double? Intercept,
    double? StdDev,
    double? RSquared,
    decimal? Line // last line segment only
) : IReusable
{
    double IReusable.Value => Slope.Null2NaN();
}
