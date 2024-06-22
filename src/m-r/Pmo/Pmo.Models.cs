namespace Skender.Stock.Indicators;

public record struct PmoResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Pmo { get; set; }
    public double? Signal { get; set; }

    readonly double IReusableResult.Value
        => Pmo.Null2NaN();
}
