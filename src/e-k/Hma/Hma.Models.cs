namespace Skender.Stock.Indicators;

[Serializable]
public sealed class HmaResult : ResultBase, IReusableResult
{
    public HmaResult(DateTime date)
    {
        Date = date;
    }

    public double? Hma { get; set; }

    double? IReusableResult.Value => Hma;
}
