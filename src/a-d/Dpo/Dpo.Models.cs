namespace Skender.Stock.Indicators;

[Serializable]
public class DpoResult : ResultBase, IReusableResult
{
    public DpoResult(DateTime date)
    {
        Date = date;
    }

    public double? Sma { get; set; }
    public double? Dpo { get; set; }

    double? IReusableResult.Value => Dpo;
}
