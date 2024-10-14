namespace Skender.Stock.Indicators;

public record ElderRayResult
(
    DateTime Timestamp,
    double? Ema,
    double? BullPower,
    double? BearPower
) : IReusable
{
    public double Value => (BullPower + BearPower).Null2NaN();
}
