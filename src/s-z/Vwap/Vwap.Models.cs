namespace Skender.Stock.Indicators;

public sealed record class VwapResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Vwap { get; set; }

    double IReusableResult.Value => Vwap.Null2NaN();
}
