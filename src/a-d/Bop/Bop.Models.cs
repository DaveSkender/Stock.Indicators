namespace Skender.Stock.Indicators;

public sealed class BopResult : ResultBase, IReusableResult
{
    public double? Bop { get; set; }

    double IReusableResult.Value => Bop.Null2NaN();
}
