namespace Skender.Stock.Indicators;

public sealed class CciResult : ResultBase, IReusableResult
{
    public double? Cci { get; set; }

    double IReusableResult.Value => Cci.Null2NaN();
}
