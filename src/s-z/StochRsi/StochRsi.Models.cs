namespace Skender.Stock.Indicators;

[Serializable]
public class StochRsiResult : ResultBase
{
    public double? StochRsi { get; set; }
    public double? Signal { get; set; }
}
