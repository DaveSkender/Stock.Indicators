namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Slope calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the data point.</param>
/// <param name="Slope">The value of the slope at this point.</param>
/// <param name="Intercept">The intercept value at this point.</param>
/// <param name="StdDev">The standard deviation value at this point.</param>
/// <param name="RSquared">The R-squared value at this point.</param>
/// <param name="Line">The value of the last line segment only.</param>
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
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Slope.Null2NaN();
}
