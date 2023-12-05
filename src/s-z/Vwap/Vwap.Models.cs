namespace Skender.Stock.Indicators;

public sealed class VwapResult : ResultBase, IReusableResult
{
    public double? Vwap { get; set; }

    double IReusableResult.Value => Vwap.Null2NaN();
}
