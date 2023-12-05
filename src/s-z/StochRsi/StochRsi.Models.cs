namespace Skender.Stock.Indicators;

public sealed class StochRsiResult : ResultBase, IReusableResult
{
    public double? StochRsi { get; set; }
    public double? Signal { get; set; }

    double IReusableResult.Value => StochRsi.Null2NaN();
}
