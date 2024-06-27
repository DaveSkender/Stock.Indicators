namespace Skender.Stock.Indicators;

public record struct ElderRayResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Ema { get; set; }
    public double? BullPower { get; set; }
    public double? BearPower { get; set; }

    readonly double IReusable.Value
        => (BullPower + BearPower).Null2NaN();
}
