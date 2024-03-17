namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class PvoResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Pvo { get; set; }
    public double? Signal { get; set; }
    public double? Histogram { get; set; }

    double IReusableResult.Value => Pvo.Null2NaN();
}
