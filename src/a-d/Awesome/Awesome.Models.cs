namespace Skender.Stock.Indicators;

public record struct AwesomeResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Oscillator { get; set; }
    public double? Normalized { get; set; }

    readonly double IReusableResult.Value
        => Oscillator.Null2NaN();
}
