namespace Skender.Stock.Indicators;

public record struct CorrResult
(
    DateTime Timestamp,
    double? VarianceA = null,
    double? VarianceB = null,
    double? Covariance = null,
    double? Correlation = null,
    double? RSquared = null
) : IReusable
{
    readonly double IReusable.Value
        => Correlation.Null2NaN();
}
