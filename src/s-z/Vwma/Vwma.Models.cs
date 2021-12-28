namespace Skender.Stock.Indicators;

[Serializable]
public class VwmaResult : ResultBase
{
    public decimal Volume { get; set; }   // for reference only
    public decimal? Vwma { get; set; }  // simple moving average of volume
}
