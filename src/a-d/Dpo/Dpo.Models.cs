namespace Skender.Stock.Indicators;

[Serializable]
public class DpoResult : ResultBase
{
    public double? Sma { get; set; }
    public double? Dpo { get; set; }
}
