namespace Skender.Stock.Indicators;

[Serializable]
public sealed class PmoResult : ResultBase, IReusableResult
{
    public PmoResult(DateTime date)
    {
        Date = date;
    }

    public double? Pmo { get; set; }
    public double? Signal { get; set; }

    double IReusableResult.Value => Pmo.Null2NaN();
}
