namespace Skender.Stock.Indicators;

public readonly record struct BetaResult(
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
    double IReusable.Value => Beta.Null2NaN();
}

public enum BetaType
{
    Standard,
    Up,
    Down,
    All
}
