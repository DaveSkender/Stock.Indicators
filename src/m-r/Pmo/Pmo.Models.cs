namespace Skender.Stock.Indicators;

[Serializable]
public class PmoResult : ResultBase
{
    public double? Pmo { get; set; }
    public double? Signal { get; set; }

    // internal use only
    internal double? RocEma { get; set; }
}
