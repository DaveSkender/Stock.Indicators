namespace Skender.Stock.Indicators;

public record CorrResult
(
    DateTime Timestamp,
    double? VarianceA = null,
    double? VarianceB = null,
    double? Covariance = null,
    double? Correlation = null,
    double? RSquared = null
) : Reusable(Timestamp)
{
    public override double Value => Correlation.Null2NaN();
}
