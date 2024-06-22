namespace Skender.Stock.Indicators;

public record struct RsiResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Rsi { get; set; }

    readonly double IReusableResult.Value
        => Rsi.Null2NaN();
}
