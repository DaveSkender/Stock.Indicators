namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Volume Weighted Moving Average (VWMA) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the data point.</param>
/// <param name="Vwma">The value of the VWMA at this point.</param>
[Serializable]
public record VwmaResult
(
    DateTime Timestamp,
    double? Vwma
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Vwma.Null2NaN();
}
