namespace Skender.Stock.Indicators;

public record struct SmiResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Smi { get; set; }
    public double? Signal { get; set; }

    readonly double IReusableResult.Value
        => Smi.Null2NaN();
}
