namespace Skender.Stock.Indicators;

public sealed class DynamicResult : ResultBase, IReusableResult
{
    public double? Dynamic { get; set; }

    double IReusableResult.Value => Dynamic.Null2NaN();
}
