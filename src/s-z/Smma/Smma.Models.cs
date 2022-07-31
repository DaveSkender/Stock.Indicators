namespace Skender.Stock.Indicators;

[Serializable]
public sealed class SmmaResult : ResultBase, IReusableResult
{
    public SmmaResult(DateTime date)
    {
        Date = date;
    }

    public double? Smma { get; set; }

    double? IReusableResult.Value => Smma;
}
