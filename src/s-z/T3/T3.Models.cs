namespace Skender.Stock.Indicators;

public sealed class T3Result : ResultBase, IReusableResult
{
    public double? T3 { get; set; }

    double IReusableResult.Value => T3.Null2NaN();
}
