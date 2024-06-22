namespace Skender.Stock.Indicators;

public record struct StochRsiResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? StochRsi { get; set; }
    public double? Signal { get; set; }

    readonly double IReusableResult.Value
        => StochRsi.Null2NaN();
}
