namespace Skender.Stock.Indicators;

[Serializable]
public sealed class VwmaResult : ResultBase, IReusableResult
{
    public VwmaResult(DateTime date)
    {
        Date = date;
    }

    public double? Vwma { get; set; }

    double? IReusableResult.Value => Vwma;
}
