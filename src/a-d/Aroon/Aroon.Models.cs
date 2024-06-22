namespace Skender.Stock.Indicators;

public record struct AroonResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? AroonUp { get; set; }
    public double? AroonDown { get; set; }
    public double? Oscillator { get; set; }

    readonly double IReusableResult.Value
        => Oscillator.Null2NaN();
}
