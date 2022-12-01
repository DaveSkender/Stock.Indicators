namespace Skender.Stock.Indicators;

[Serializable]
public class DynamicResult : ResultBase, IReusableResult
{
    public DynamicResult(DateTime date)
    {
        Date = date;
    }

    public double? Dynamic { get; set; }

    double? IReusableResult.Value => Dynamic;
}
