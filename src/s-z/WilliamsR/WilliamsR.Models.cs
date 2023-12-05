namespace Skender.Stock.Indicators;

public sealed class WilliamsResult : ResultBase, IReusableResult
{
    public double? WilliamsR { get; set; }

    double IReusableResult.Value => WilliamsR.Null2NaN();
}
