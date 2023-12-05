namespace Skender.Stock.Indicators;

public sealed class VwmaResult : ResultBase, IReusableResult
{
    public double? Vwma { get; set; }

    double IReusableResult.Value => Vwma.Null2NaN();
}
