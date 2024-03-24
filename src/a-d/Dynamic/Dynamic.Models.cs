namespace Skender.Stock.Indicators;

public sealed record class DynamicResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Dynamic { get; set; }

    double IReusableResult.Value => Dynamic.Null2NaN();
}
