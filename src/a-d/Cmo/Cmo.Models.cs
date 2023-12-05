namespace Skender.Stock.Indicators;

public sealed class CmoResult : ResultBase, IReusableResult
{
    public double? Cmo { get; set; }

    double IReusableResult.Value => Cmo.Null2NaN();
}
