namespace Skender.Stock.Indicators;

public record struct WilliamsResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? WilliamsR { get; set; }

    readonly double IReusableResult.Value
        => WilliamsR.Null2NaN();
}
