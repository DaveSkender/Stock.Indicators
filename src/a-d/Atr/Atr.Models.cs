namespace Skender.Stock.Indicators;

public interface IAtrResult
{
    public decimal? Tr { get; }
    public decimal? Atr { get; }
    public decimal? Atrp { get; }
}

[Serializable]
public sealed class AtrResult : ResultBase, IAtrResult, IReusableResult
{
    public decimal? Tr { get; set; }
    public decimal? Atr { get; set; }
    public decimal? Atrp { get; set; }

    double? IReusableResult.Value => (double?)Atrp;
}
