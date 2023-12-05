namespace Skender.Stock.Indicators;

public sealed class WmaResult : ResultBase, IReusableResult
{
    public double? Wma { get; set; }

    double IReusableResult.Value => Wma.Null2NaN();
}
