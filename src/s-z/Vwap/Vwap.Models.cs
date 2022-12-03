namespace Skender.Stock.Indicators;

[Serializable]
public sealed class VwapResult : ResultBase, IReusableResult
{
    public VwapResult(DateTime date)
    {
        Date = date;
    }

    public double? Vwap { get; set; }

    double? IReusableResult.Value => Vwap;
}
