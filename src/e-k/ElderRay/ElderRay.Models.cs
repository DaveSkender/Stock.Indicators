namespace Skender.Stock.Indicators;

public record struct ElderRayResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Ema { get; set; }
    public double? BullPower { get; set; }
    public double? BearPower { get; set; }

    readonly double IReusableResult.Value
        => (BullPower + BearPower).Null2NaN();
}
