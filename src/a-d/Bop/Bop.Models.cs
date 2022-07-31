namespace Skender.Stock.Indicators;

[Serializable]
public sealed class BopResult : ResultBase, IReusableResult
{
    public BopResult(DateTime date)
    {
        Date = date;
    }

    public double? Bop { get; set; }

    double? IReusableResult.Value => Bop;
}
