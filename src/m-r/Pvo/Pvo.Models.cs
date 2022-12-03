namespace Skender.Stock.Indicators;

[Serializable]
public sealed class PvoResult : ResultBase, IReusableResult
{
    public PvoResult(DateTime date)
    {
        Date = date;
    }

    public double? Pvo { get; set; }
    public double? Signal { get; set; }
    public double? Histogram { get; set; }

    double? IReusableResult.Value => Pvo;
}
