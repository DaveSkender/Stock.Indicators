namespace Skender.Stock.Indicators;

[Serializable]
public class DpoResult : ResultBase
{
    public decimal? Sma { get; set; }
    public decimal? Dpo { get; set; }
}
