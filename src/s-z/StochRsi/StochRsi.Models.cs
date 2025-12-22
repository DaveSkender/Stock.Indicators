namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Stochastic RSI calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the data point.</param>
/// <param name="StochRsi">The value of the Stochastic RSI at this point.</param>
/// <param name="Signal">The signal line value at this point.</param>
/// <remarks>
/// StochRsi and Signal are bounded between 0 and 100 when calculated.
/// </remarks>
[Serializable]
public record StochRsiResult
(
    DateTime Timestamp,
    double? StochRsi = null,
    double? Signal = null
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => StochRsi.Null2NaN();
}
