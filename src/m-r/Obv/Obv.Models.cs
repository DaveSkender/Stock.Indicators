namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of an On-Balance Volume (OBV) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="Obv">The value of the On-Balance Volume (OBV).</param>
[Serializable]
public record ObvResult
(
    DateTime Timestamp,
    double Obv
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Obv;
}
