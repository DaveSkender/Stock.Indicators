namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of the Average True Range (ATR) indicator calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="Tr">The True Range (TR) value.</param>
/// <param name="Atr">The Average True Range (ATR) value.</param>
/// <param name="Atrp">The ATR percentage value.</param>
[Serializable]
public record AtrResult
(
    DateTime Timestamp,
    double? Tr = null,
    double? Atr = null,
    double? Atrp = null
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Atrp.Null2NaN();
}
