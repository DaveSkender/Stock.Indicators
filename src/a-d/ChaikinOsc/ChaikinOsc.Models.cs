namespace Skender.Stock.Indicators;

public sealed record class ChaikinOscResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? MoneyFlowMultiplier { get; set; }
    public double? MoneyFlowVolume { get; set; }
    public double? Adl { get; set; }
    public double? Oscillator { get; set; }

    double IReusableResult.Value => Oscillator.Null2NaN();
}
