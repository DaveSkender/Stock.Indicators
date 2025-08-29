namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Hull Moving Average (HMA) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="Hma">The value of the Hull Moving Average.</param>
[Serializable]
public record HmaResult
(
    DateTime Timestamp,
    double? Hma = null
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Hma.Null2NaN();
}
