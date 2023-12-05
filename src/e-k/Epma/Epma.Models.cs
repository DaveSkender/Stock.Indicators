namespace Skender.Stock.Indicators;

public sealed class EpmaResult : ResultBase, IReusableResult
{
    public double? Epma { get; set; }

    double IReusableResult.Value => Epma.Null2NaN();
}
