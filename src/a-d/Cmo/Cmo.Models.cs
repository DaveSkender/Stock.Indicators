namespace Skender.Stock.Indicators;

[Serializable]
public sealed class CmoResult : ResultBase, IReusableResult
{
    public CmoResult(DateTime date)
    {
        Date = date;
    }

    public double? Cmo { get; set; }

    double? IReusableResult.Value => Cmo;
}
