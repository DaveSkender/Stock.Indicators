namespace Skender.Stock.Indicators;

public record struct BopResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Bop { get; set; }

    readonly double IReusableResult.Value
        => Bop.Null2NaN();
}
