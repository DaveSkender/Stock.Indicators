namespace Skender.Stock.Indicators;

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
    public double Value => (BullPower + BearPower).Null2NaN();
}
