namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class StochRsiResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? StochRsi { get; set; }
    public double? Signal { get; set; }

    double IReusableResult.Value => StochRsi.Null2NaN();
}
