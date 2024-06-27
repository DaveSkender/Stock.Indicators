namespace Skender.Stock.Indicators;

public record struct BopResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Bop { get; set; }

    readonly double IReusable.Value
        => Bop.Null2NaN();
}
