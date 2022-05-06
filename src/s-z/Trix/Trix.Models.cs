namespace Skender.Stock.Indicators;

[Serializable]
public class TrixResult : ResultBase
{
    public double? Ema3 { get; set; }
    public double? Trix { get; set; }
    public double? Signal { get; set; }
}
