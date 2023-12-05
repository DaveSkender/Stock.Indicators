namespace Skender.Stock.Indicators;

public sealed class ForceIndexResult : ResultBase, IReusableResult
{
    public double? ForceIndex { get; set; }

    double IReusableResult.Value => ForceIndex.Null2NaN();
}
