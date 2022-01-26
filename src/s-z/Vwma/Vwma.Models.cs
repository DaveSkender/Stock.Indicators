namespace Skender.Stock.Indicators;

[Serializable]
public class VwmaResult : ResultBase
{
    public decimal? Vwma { get; set; } // simple moving average of volume
}
