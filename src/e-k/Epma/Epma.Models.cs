namespace Skender.Stock.Indicators;

[Serializable]
public sealed class EpmaResult : ResultBase, IReusableResult
{
    public EpmaResult(DateTime date)
    {
        Date = date;
    }

    public double? Epma { get; set; }

    double? IReusableResult.Value => Epma;
}
