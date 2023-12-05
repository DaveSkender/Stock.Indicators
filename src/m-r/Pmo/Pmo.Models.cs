namespace Skender.Stock.Indicators;

public sealed class PmoResult : ResultBase, IReusableResult
{
    public double? Pmo { get; set; }
    public double? Signal { get; set; }

    double IReusableResult.Value => Pmo.Null2NaN();
}
