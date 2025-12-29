namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Smoothed Moving Average (SMMA) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the data point.</param>
/// <param name="Smma">The value of the SMMA at this point.</param>
[Serializable]
public record SmmaResult
(
    DateTime Timestamp,
    double? Smma = null
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Smma.Null2NaN();
}
