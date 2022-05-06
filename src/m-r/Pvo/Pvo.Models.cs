namespace Skender.Stock.Indicators;

[Serializable]
public class PvoResult : ResultBase
{
    public double? Pvo { get; set; }
    public double? Signal { get; set; }
    public double? Histogram { get; set; }
}
