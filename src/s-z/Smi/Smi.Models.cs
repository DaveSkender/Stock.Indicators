namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class SmiResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Smi { get; set; }
    public double? Signal { get; set; }

    double IReusableResult.Value => Smi.Null2NaN();
}
