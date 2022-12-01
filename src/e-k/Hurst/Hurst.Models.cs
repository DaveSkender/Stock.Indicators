namespace Skender.Stock.Indicators;

[Serializable]
public class HurstResult : ResultBase, IReusableResult
{
    public HurstResult(DateTime date)
    {
        Date = date;
    }

    public double? HurstExponent { get; set; }

    double? IReusableResult.Value => HurstExponent;
}
