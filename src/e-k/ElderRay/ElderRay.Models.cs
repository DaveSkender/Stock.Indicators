namespace Skender.Stock.Indicators;

public record ElderRayResult
(
    DateTime Timestamp,
    double? Ema,
    double? BullPower,
    double? BearPower
) : Reusable(Timestamp)
{
    public override double Value => (BullPower + BearPower).Null2NaN();
}
