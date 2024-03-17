namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class EpmaResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Epma { get; set; }

    double IReusableResult.Value => Epma.Null2NaN();
}
