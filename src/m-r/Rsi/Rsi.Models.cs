namespace Skender.Stock.Indicators;

[Serializable]
public sealed class RsiResult : ResultBase, IReusableResult
{
    public double? Rsi { get; set; }

    double? IReusableResult.Value => Rsi;
}
