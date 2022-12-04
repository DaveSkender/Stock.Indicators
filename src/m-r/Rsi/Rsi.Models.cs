namespace Skender.Stock.Indicators;

[Serializable]
public sealed class RsiResult : ResultBase, IReusableResult
{
    public RsiResult(DateTime date)
    {
        Date = date;
    }

    public double? Rsi { get; set; }

    double? IReusableResult.ChainValue => Rsi;
}
