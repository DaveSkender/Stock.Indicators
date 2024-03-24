namespace Skender.Stock.Indicators;

public sealed record class ForceIndexResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? ForceIndex { get; set; }

    double IReusableResult.Value => ForceIndex.Null2NaN();
}
