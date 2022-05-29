namespace Skender.Stock.Indicators;

[Serializable]
public sealed class AtrResult : ResultBase, IReusableResult
{
    public decimal? Tr { get; set; }
    public decimal? Atr { get; set; }
    public decimal? Atrp { get; set; }

    double? IReusableResult.Value => (double?)Atrp;
}
