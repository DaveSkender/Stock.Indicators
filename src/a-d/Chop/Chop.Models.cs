namespace Skender.Stock.Indicators;

[Serializable]
public class ChopResult : ResultBase, IReusableResult
{
    public ChopResult(DateTime date)
    {
        Date = date;
    }

    public double? Chop { get; set; }

    double? IReusableResult.Value => Chop;
}
