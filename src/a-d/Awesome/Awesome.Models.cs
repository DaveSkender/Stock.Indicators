namespace Skender.Stock.Indicators;

[Serializable]
public class AwesomeResult : ResultBase
{
    public double? Oscillator { get; set; }
    public double? Normalized { get; set; }
}
