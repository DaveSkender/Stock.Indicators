namespace Skender.Stock.Indicators;

[Serializable]
public sealed class AdxResult : ResultBase, IReusableResult
{
    public AdxResult(DateTime date)
    {
        Date = date;
    }

    public double? Pdi { get; set; }
    public double? Mdi { get; set; }
    public double? Adx { get; set; }
    public double? Adxr { get; set; }

    double? IReusableResult.Value => Adx;
}
