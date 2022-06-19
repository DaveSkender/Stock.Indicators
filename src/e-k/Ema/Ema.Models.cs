namespace Skender.Stock.Indicators;

[Serializable]
public sealed class EmaResult : ResultBase, IReusableResult
{
    public double? Ema { get; set; }

    double? IReusableResult.Value => Ema;
}
