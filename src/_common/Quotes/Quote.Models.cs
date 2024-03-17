namespace Skender.Stock.Indicators;

// QUOTE MODELS

public interface IQuote : ISeries, IEquatable<IQuote>
{
    decimal Open { get; }
    decimal High { get; }
    decimal Low { get; }
    decimal Close { get; }
    decimal Volume { get; }

    // CS0567 C# Interfaces cannot contain conversion,
    // equality, or inequality operators (i.e. == or !=)
    // and cannot be inforced here
}

/// <summary>
/// Built-in Quote type.
/// </summary>
[Serializable]
public record class Quote : IQuote
{
    public DateTime Timestamp { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }

    // this is only an appropriate
    // implementation for record types
    public bool Equals(IQuote? other)
      => base.Equals(other);
}

public abstract class EquatableQuote : IQuote
{
    public virtual DateTime Timestamp { get; set; }
    public virtual decimal Open { get; set; }
    public virtual decimal High { get; set; }
    public virtual decimal Low { get; set; }
    public virtual decimal Close { get; set; }
    public virtual decimal Volume { get; set; }

    public override bool Equals(object? obj)
        => Equals(obj as IQuote);

    public bool Equals(IQuote? other)
    {
        if (other is null)
        {
            return false;
        }

        // same object reference
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        // mismatch object types
        if (GetType() != other.GetType())
        {
            return false;
        }

        // deep compare
        return Timestamp == other.Timestamp
            && Open == other.Open
            && High == other.High
            && Low == other.Low
            && Close == other.Close
            && Volume == other.Volume;
    }

    public static bool operator
        ==(EquatableQuote lhs, EquatableQuote rhs)
    {
        if (lhs is null)
        {
            if (rhs is null)
            {
                // null == null = true
                return true;
            }

            // left side is null
            return false;
        }

        // null on right side also handled
        return lhs.Equals(rhs);
    }

    public static bool operator
        !=(EquatableQuote lhs, EquatableQuote rhs)
    {
        return !(lhs == rhs);
    }

    public override int GetHashCode()
    => HashCode.Combine(
            Timestamp, Open, High, Low, Close, Volume);
}

[Serializable]
internal class QuoteD
{
    internal DateTime Timestamp { get; set; }
    internal double Open { get; set; }
    internal double High { get; set; }
    internal double Low { get; set; }
    internal double Close { get; set; }
    internal double Volume { get; set; }
}
