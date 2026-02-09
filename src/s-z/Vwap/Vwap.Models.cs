namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Volume Weighted Average Price (VWAP) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the data point.</param>
/// <param name="Vwap">The value of the VWAP at this point.</param>
[Serializable]
public record VwapResult
(
   DateTime Timestamp,
   double? Vwap
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Vwap.Null2NaN();
}
