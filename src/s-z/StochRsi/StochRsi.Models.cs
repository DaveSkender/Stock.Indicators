namespace Skender.Stock.Indicators;

public record struct StochRsiResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? StochRsi { get; set; }
    public double? Signal { get; set; }

    readonly double IReusable.Value
        => StochRsi.Null2NaN();
}
