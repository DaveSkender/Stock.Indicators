namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Standard Deviation calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the data point.</param>
/// <param name="StdDev">The value of the standard deviation at this point.</param>
/// <param name="Mean">The mean value at this point.</param>
/// <param name="ZScore">The Z-score value at this point.</param>
[Serializable]
public record StdDevResult
(
    DateTime Timestamp,
    double? StdDev,
    double? Mean,
    double? ZScore
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => StdDev.Null2NaN();
}
