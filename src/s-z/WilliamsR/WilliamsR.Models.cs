namespace Skender.Stock.Indicators;

public sealed record class WilliamsResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? WilliamsR { get; set; }

    double IReusableResult.Value => WilliamsR.Null2NaN();
}
