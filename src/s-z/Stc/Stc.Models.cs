namespace Skender.Stock.Indicators;

[Serializable]
public sealed class StcResult : ResultBase, IReusableResult
{
    public StcResult(DateTime date)
    {
        Date = date;
    }

    public double? Stc { get; set; }

    double? IReusableResult.Value => Stc;
}
