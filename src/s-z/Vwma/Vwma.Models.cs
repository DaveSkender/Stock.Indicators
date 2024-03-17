namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class VwmaResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Vwma { get; set; }

    double IReusableResult.Value => Vwma.Null2NaN();
}
