namespace Skender.Stock.Indicators;

public sealed class PvoResult : ResultBase, IReusableResult
{
    public double? Pvo { get; set; }
    public double? Signal { get; set; }
    public double? Histogram { get; set; }

    double IReusableResult.Value => Pvo.Null2NaN();
}
