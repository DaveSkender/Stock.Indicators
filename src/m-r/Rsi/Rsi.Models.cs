namespace Skender.Stock.Indicators;

public sealed class RsiResult : ResultBase, IReusableResult
{
    public double? Rsi { get; set; }

    double IReusableResult.Value => Rsi.Null2NaN();
}
