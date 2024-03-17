namespace Skender.Stock.Indicators;

public sealed record class AwesomeResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Oscillator { get; set; }
    public double? Normalized { get; set; }

    double IReusableResult.Value => Oscillator.Null2NaN();
}
