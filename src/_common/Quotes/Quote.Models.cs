namespace Skender.Stock.Indicators;

// QUOTE MODELS

public interface IQuote : ISeries
{
    decimal Open { get; }
    decimal High { get; }
    decimal Low { get; }
    decimal Close { get; }
    decimal Volume { get; }
}

public class Quote : IQuote
{
    public DateTime Date { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }
}

internal class QuoteD
{
    internal DateTime Date { get; set; }
    internal double Open { get; set; }
    internal double High { get; set; }
    internal double Low { get; set; }
    internal double Close { get; set; }
    internal double Volume { get; set; }
}
