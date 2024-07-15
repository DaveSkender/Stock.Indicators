namespace Skender.Stock.Indicators;

public record SlopeResult
(
    DateTime Timestamp,
    double? Slope = null,
    double? Intercept = null,
    double? StdDev = null,
    double? RSquared = null,
    decimal? Line = null // last line segment only
) : Reusable(Timestamp, Slope.Null2NaN());
