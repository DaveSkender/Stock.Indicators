using System.Diagnostics.CodeAnalysis;

namespace Skender.Stock.Indicators;

// QUOTE MODELS

public interface IQuote
{
    public DateTime Date { get; }
    public decimal Open { get; }
    public decimal High { get; }
    public decimal Low { get; }
    public decimal Close { get; }
    public decimal Volume { get; }
}

[Serializable]
public class Quote : IQuote
{
    public DateTime Date { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }
}

[Serializable]
internal class QuoteD
{
    internal DateTime Date { get; set; }
    internal double Open { get; set; }
    internal double High { get; set; }
    internal double Low { get; set; }
    internal double Close { get; set; }
    internal double Volume { get; set; }
}

[SuppressMessage(
    "Performance",
    "CA1815:Override equals and operator equals on value types",
    Justification = "Not ready to add.")]
public struct Price
{
    public Price(DateTime date, double value)
    {
        Date = date;
        Value = value;
    }

    public DateTime Date { get; internal set; }
    public double Value { get; internal set; }

    public override string ToString()
    {
        return $"{Date} | {Value}";
    }
}