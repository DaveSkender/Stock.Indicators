namespace Skender.Stock.Indicators;

public record struct ChaikinOscResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? MoneyFlowMultiplier { get; set; }
    public double? MoneyFlowVolume { get; set; }
    public double? Adl { get; set; }
    public double? Oscillator { get; set; }

    readonly double IReusableResult.Value
        => Oscillator.Null2NaN();
}
