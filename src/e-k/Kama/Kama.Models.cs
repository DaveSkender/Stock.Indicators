namespace Skender.Stock.Indicators;

public sealed class KamaResult : ResultBase, IReusableResult
{
    public double? ER { get; set; }
    public double? Kama { get; set; }

    double IReusableResult.Value => Kama.Null2NaN();
}
