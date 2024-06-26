namespace Skender.Stock.Indicators;

public record struct AlmaResult(
    DateTime Timestamp,
    double? Alma) : IReusableResult
{
    readonly double IReusableResult.Value
        => Alma.Null2NaN();
}
