namespace Skender.Stock.Indicators;

[Serializable]
public class HeikinAshiResult : ResultBase, IQuote
{
    public decimal? Open { get; set; }
    public decimal? High { get; set; }
    public decimal? Low { get; set; }
    public decimal? Close { get; set; }
    public decimal? Volume { get; set; }
    public decimal OHLC4 { get; set; }
}
