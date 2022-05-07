namespace Skender.Stock.Indicators;

// QUOTE MODELS

public interface IQuote
{
    public DateTime Date { get; }
    public decimal? Open { get; }
    public decimal? High { get; }
    public decimal? Low { get; }
    public decimal? Close { get; }
    public decimal? Volume { get; }
    public decimal OHLC4 { get; set; }
}

[Serializable]
public class Quote : IQuote
{
    public DateTime Date { get; set; }
    public decimal? Open { get; set; }
    public decimal? High { get; set; }
    public decimal? Low { get; set; }
    public decimal? Close { get; set; }
    public decimal? Volume { get; set; }
    public decimal OHLC4 { get; set; }
}

[Serializable]
internal class QuoteD
{
    internal DateTime Date { get; set; }
    internal double? Open { get; set; }
    internal double? High { get; set; }
    internal double? Low { get; set; }
    internal double? Close { get; set; }
    internal double? Volume { get; set; }
    internal double? OHLC4 { get; set; }
}
