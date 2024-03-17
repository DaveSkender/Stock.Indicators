namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class StcResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Stc { get; set; }

    double IReusableResult.Value => Stc.Null2NaN();
}
