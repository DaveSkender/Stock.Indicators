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

public struct SimplePrice : IEquatable<SimplePrice>
{
    public SimplePrice(DateTime date, double value)
    {
        Date = date;
        Value = value;
    }

    public DateTime Date { get; internal set; }
    public double Value { get; internal set; }

    public static bool operator ==(SimplePrice left, SimplePrice right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SimplePrice left, SimplePrice right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return $"{Date} | {Value}";
    }

    public override int GetHashCode()
    {
        return (int)(Date.Ticks * (int)Value);
    }

    public override bool Equals(object? obj)
    {
        return obj is SimplePrice price && Equals(price);
    }

    public bool Equals(SimplePrice other)
    {
        return Date == other.Date && Value == other.Value;
    }
}