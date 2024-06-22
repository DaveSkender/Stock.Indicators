namespace Skender.Stock.Indicators;

public record struct KvoResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Oscillator { get; set; }
    public double? Signal { get; set; }

    readonly double IReusableResult.Value
        => Oscillator.Null2NaN();
}
