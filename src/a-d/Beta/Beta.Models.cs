namespace Skender.Stock.Indicators;

public record struct BetaResult(
    DateTime Timestamp,
    double? Beta = null,
    double? BetaUp = null,
    double? BetaDown = null,
    double? Ratio = null,
    double? Convexity = null,
    double? ReturnsEval = null,
    double? ReturnsMrkt = null
) : IReusable
{
    readonly double IReusable.Value
        => Beta.Null2NaN();
}

public enum BetaType
{
    Standard,
    Up,
    Down,
    All
}
