namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class KamaResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? ER { get; set; }
    public double? Kama { get; set; }

    double IReusableResult.Value => Kama.Null2NaN();
}
