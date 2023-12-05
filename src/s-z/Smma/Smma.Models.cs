namespace Skender.Stock.Indicators;

public sealed class SmmaResult : ResultBase, IReusableResult
{
    public double? Smma { get; set; }

    double IReusableResult.Value => Smma.Null2NaN();
}
