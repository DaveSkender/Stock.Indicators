namespace Skender.Stock.Indicators;

public record struct TsiResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Tsi { get; set; }
    public double? Signal { get; set; }

    readonly double IReusableResult.Value
        => Tsi.Null2NaN();
}
