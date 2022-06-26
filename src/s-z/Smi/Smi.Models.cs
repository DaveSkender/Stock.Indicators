namespace Skender.Stock.Indicators;

[Serializable]
public sealed class SmiResult : ResultBase, IReusableResult
{
    public SmiResult(DateTime date)
    {
        Date = date;
    }

    public double? Smi { get; set; }
    public double? Signal { get; set; }

    double? IReusableResult.Value => Smi;
}
