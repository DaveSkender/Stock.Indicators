namespace Skender.Stock.Indicators;

[Serializable]
public class WmaResult : ResultBase, IReusableResult
{
    public WmaResult(DateTime date)
    {
        Date = date;
    }

    public double? Wma { get; set; }

    double? IReusableResult.Value => Wma;
}
