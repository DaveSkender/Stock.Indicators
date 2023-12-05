namespace Skender.Stock.Indicators;

public sealed class TsiResult : ResultBase, IReusableResult
{
    public double? Tsi { get; set; }
    public double? Signal { get; set; }

    double IReusableResult.Value => Tsi.Null2NaN();
}
