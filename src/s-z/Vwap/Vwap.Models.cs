namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class VwapResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Vwap { get; set; }

    double IReusableResult.Value => Vwap.Null2NaN();
}
