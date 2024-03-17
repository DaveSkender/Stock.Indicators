namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class MamaResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Mama { get; set; }
    public double? Fama { get; set; }

    double IReusableResult.Value => Mama.Null2NaN();
}
