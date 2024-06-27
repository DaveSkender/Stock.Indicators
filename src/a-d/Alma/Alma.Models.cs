namespace Skender.Stock.Indicators;

public record struct AlmaResult(
    DateTime Timestamp,
    double? Alma) : IReusable
{
    readonly double IReusable.Value
        => Alma.Null2NaN();
}
