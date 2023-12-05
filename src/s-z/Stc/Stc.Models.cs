namespace Skender.Stock.Indicators;

public sealed class StcResult : ResultBase, IReusableResult
{
    public double? Stc { get; set; }

    double IReusableResult.Value => Stc.Null2NaN();
}
