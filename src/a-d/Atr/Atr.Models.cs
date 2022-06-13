namespace Skender.Stock.Indicators;

[Serializable]
public sealed class AtrResult : ResultBase, IReusableResult
{
    public double? Tr { get; set; }
    public double? Atr { get; set; }
    public double? Atrp { get; set; }

    double? IReusableResult.Value => Atrp;
}
