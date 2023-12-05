namespace Skender.Stock.Indicators;

public sealed class AlmaResult : ResultBase, IReusableResult
{
    public double? Alma { get; set; }

    double IReusableResult.Value => Alma.Null2NaN();
}
