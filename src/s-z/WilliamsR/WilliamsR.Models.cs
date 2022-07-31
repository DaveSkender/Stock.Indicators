namespace Skender.Stock.Indicators;

[Serializable]
public sealed class WilliamsResult : ResultBase, IReusableResult
{
    public WilliamsResult(DateTime date)
    {
        Date = date;
    }

    public double? WilliamsR { get; set; }

    double? IReusableResult.Value => WilliamsR;
}
