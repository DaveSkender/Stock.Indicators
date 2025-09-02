namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of the correlation calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="VarianceA">The variance of series A.</param>
/// <param name="VarianceB">The variance of series B.</param>
/// <param name="Covariance">The covariance between series A and B.</param>
/// <param name="Correlation">The correlation coefficient between series A and B.</param>
/// <param name="RSquared">The R-squared value of the correlation.</param>
[Serializable]
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
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Correlation.Null2NaN();
}
