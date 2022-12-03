namespace Skender.Stock.Indicators;

[Serializable]
public sealed class ParabolicSarResult : ResultBase, IReusableResult
{
    public ParabolicSarResult(DateTime date)
    {
        Date = date;
    }

    public double? Sar { get; set; }
    public bool? IsReversal { get; set; }

    double? IReusableResult.Value => Sar;
}
