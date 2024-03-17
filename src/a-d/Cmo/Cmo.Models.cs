namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class CmoResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Cmo { get; set; }

    double IReusableResult.Value => Cmo.Null2NaN();
}
