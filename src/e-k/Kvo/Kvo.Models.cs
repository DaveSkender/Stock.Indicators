namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class KvoResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Oscillator { get; set; }
    public double? Signal { get; set; }

    double IReusableResult.Value => Oscillator.Null2NaN();
}
