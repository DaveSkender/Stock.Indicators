namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class HurstResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? HurstExponent { get; set; }

    double IReusableResult.Value => HurstExponent.Null2NaN();
}
