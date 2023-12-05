namespace Skender.Stock.Indicators;

public sealed class EmaResult : ResultBase, IReusableResult
{
    public double? Ema { get; set; }

    double IReusableResult.Value => Ema.Null2NaN();
}
