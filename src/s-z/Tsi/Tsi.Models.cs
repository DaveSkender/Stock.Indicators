namespace Skender.Stock.Indicators;

[Serializable]
public class TsiResult : ResultBase
{
    public double? Tsi { get; set; }
    public double? Signal { get; set; }
}
