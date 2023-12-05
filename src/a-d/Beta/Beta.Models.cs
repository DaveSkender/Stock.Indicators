namespace Skender.Stock.Indicators;

public sealed class BetaResult : ResultBase, IReusableResult
{
    public BetaResult(DateTime date)
    {
        Date = date;
    }

    public double? Beta { get; set; }
    public double? BetaUp { get; set; }
    public double? BetaDown { get; set; }
    public double? Ratio { get; set; }
    public double? Convexity { get; set; }
    public double? ReturnsEval { get; set; }
    public double? ReturnsMrkt { get; set; }

    double IReusableResult.Value => Beta.Null2NaN();
}

public enum BetaType
{
    Standard,
    Up,
    Down,
    All
}
