namespace Skender.Stock.Indicators;

public record CorrResult
(
    DateTime Timestamp,
    double? VarianceA = null,
    double? VarianceB = null,
    double? Covariance = null,
    double? Correlation = null,
    double? RSquared = null
) : IReusable
{
    public double Value => Correlation.Null2NaN();
}
