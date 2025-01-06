namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Weighted Moving Average (WMA) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the data point.</param>
/// <param name="Wma">The value of the WMA at this point.</param>
[Serializable]
public record WmaResult
(
    DateTime Timestamp,
    double? Wma
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Wma.Null2NaN();
}
