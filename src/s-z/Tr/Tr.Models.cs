namespace Skender.Stock.Indicators;

public sealed class TrResult : ResultBase, IReusableResult
{
    public double? Tr { get; set; }

    double IReusableResult.Value => Tr.Null2NaN();
}
