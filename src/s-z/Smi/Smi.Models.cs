namespace Skender.Stock.Indicators;

public record struct SmiResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Smi { get; set; }
    public double? Signal { get; set; }

    readonly double IReusable.Value
        => Smi.Null2NaN();
}
