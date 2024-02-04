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
    // and cannot be inforces here
}

public record class Quote : IQuote
{
    public DateTime TickDate { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }

    public bool Equals(IQuote? other)
      => base.Equals(other);
}

public abstract class EquatableQuote<TQuote>()
    where TQuote : IQuote
{
    // IMPORTANT: this does not override the general Equals(objA, objB),
    // so don't use it for equality comparison.

    public virtual DateTime TickDate { get; set; }
    public virtual decimal Open { get; set; }
    public virtual decimal High { get; set; }
    public virtual decimal Low { get; set; }
    public virtual decimal Close { get; set; }
    public virtual decimal Volume { get; set; }

    public override bool Equals(object? obj)
        => this.Equals(obj);

    public bool Equals(IQuote? other)
        => Equals(other);

    public bool Equals(TQuote? other)
    {

        if (other is null)
        {
            return false;
        }

        // Optimization for a common success case.
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        // If run-time types are not exactly the same, return false.
        if (GetType() != other.GetType())
        {
            return false;
        }

        // Return true if the fields match.
        // Note that the base class is not invoked because it is
        // System.Object, which defines Equals as reference equality.
        return (TickDate == other.TickDate)
            && (Open == other.Open)
            && (High == other.High)
            && (Low == other.Low)
            && (Close == other.Close)
            && (Volume == other.Volume);
    }

    public static bool operator
        ==(EquatableQuote<TQuote> lhs, EquatableQuote<TQuote> rhs)
    {
        if (lhs is null)
        {
            if (rhs is null)
            {
                // null == null = true.
                return true;
            }

            // Only the left side is null.
            return false;
        }
        // Equals handles the case of null on right side.
        return lhs.Equals(rhs);
    }

    public static bool operator
        !=(EquatableQuote<TQuote> lhs, EquatableQuote<TQuote> rhs)
          => !(lhs == rhs);

    public override int GetHashCode()
        => HashCode.Combine(
            TickDate, Open, High, Low, Close, Volume);
}

internal class QuoteD
{
    internal DateTime TickDate { get; set; }
    internal double Open { get; set; }
    internal double High { get; set; }
    internal double Low { get; set; }
    internal double Close { get; set; }
    internal double Volume { get; set; }
}
