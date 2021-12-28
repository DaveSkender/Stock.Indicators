namespace Skender.Stock.Indicators;

[Serializable]
public class KvoResult : ResultBase
{
    public double? Oscillator { get; set; }
    public double? Signal { get; set; }
}
