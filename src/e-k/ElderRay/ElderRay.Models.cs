namespace Skender.Stock.Indicators;

public record ElderRayResult
(
    DateTime Timestamp,
    double? Ema,
    double? BullPower,
    double? BearPower
) : Reusable(Timestamp, (BullPower + BearPower).Null2NaN());
