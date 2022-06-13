namespace Skender.Stock.Indicators;

[Serializable]
public sealed class TrixResult : ResultBase, IReusableResult
{
    public double? Ema3 { get; set; }
    public double? Trix { get; set; }
    public double? Signal { get; set; }

    double? IReusableResult.Value => Trix;
}
