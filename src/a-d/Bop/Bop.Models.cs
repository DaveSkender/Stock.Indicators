namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class BopResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Bop { get; set; }

    double IReusableResult.Value => Bop.Null2NaN();
}
