namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of an Elder Ray calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="Ema">The Exponential Moving Average (EMA) value.</param>
/// <param name="BullPower">The Bull Power value.</param>
/// <param name="BearPower">The Bear Power value.</param>
[Serializable]
public record ElderRayResult
(
    DateTime Timestamp,
    double? Ema,
    double? BullPower,
    double? BearPower
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => (BullPower + BearPower).Null2NaN();
}
