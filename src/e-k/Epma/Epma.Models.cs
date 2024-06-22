namespace Skender.Stock.Indicators;

public record struct EpmaResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Epma { get; set; }

    readonly double IReusableResult.Value
        => Epma.Null2NaN();
}
