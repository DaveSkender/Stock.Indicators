namespace Skender.Stock.Indicators;

public record struct KvoResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Oscillator { get; set; }
    public double? Signal { get; set; }

    readonly double IReusable.Value
        => Oscillator.Null2NaN();
}
