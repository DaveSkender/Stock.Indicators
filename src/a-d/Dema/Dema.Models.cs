namespace Skender.Stock.Indicators;

public sealed class DemaResult : ResultBase, IReusableResult
{
    public double? Dema { get; set; }

    double IReusableResult.Value => Dema.Null2NaN();
}
