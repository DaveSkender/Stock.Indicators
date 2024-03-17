namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class DpoResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Sma { get; set; }
    public double? Dpo { get; set; }

    double IReusableResult.Value => Dpo.Null2NaN();
}
