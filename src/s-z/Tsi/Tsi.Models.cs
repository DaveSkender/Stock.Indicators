namespace Skender.Stock.Indicators;

[Serializable]
public class TsiResult : ResultBase, IReusableResult
{
    public TsiResult(DateTime date)
    {
        Date = date;
    }

    public double? Tsi { get; set; }
    public double? Signal { get; set; }

    double? IReusableResult.Value => Tsi;
}
