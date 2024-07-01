namespace Skender.Stock.Indicators;

public readonly record struct ElderRayResult
(
    DateTime Timestamp,
    double? Ema,
    double? BullPower,
    double? BearPower
) : IReusable
{
    double IReusable.Value => (BullPower + BearPower).Null2NaN();
}
