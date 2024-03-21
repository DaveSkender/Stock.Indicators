namespace Skender.Stock.Indicators;

// QUOTE MODELS

/// <summary>
/// Quote interface can be used to modify or create your own quote class
/// for compatibility to this library.
/// </summary>
public interface IQuote : ISeries
{
    /// <summary>
    /// Opening ticker price of this bar
    /// </summary>
    decimal Open { get; }

    /// <summary>
    /// High ticker price of this bar
    /// </summary>
    decimal High { get; }

    /// <summary>
    /// Low ticker price of this bar
    /// </summary>
    decimal Low { get; }

    /// <summary>
    /// Last ticker price of this bar
    /// </summary>
    decimal Close { get; }

    /// <summary>
    /// Quantity of units transacted during this bar
    /// </summary>
    decimal Volume { get; }
}

/// <summary>
/// Built-in Quote class can be used for providing quotes to this library.
/// This is a good choice if you have no current intention of extending it.
/// </summary>
public record class Quote : IQuote
{
    /// <inheritdoc/>
    public DateTime Date { get; set; }

    /// <inheritdoc/>
    public decimal Open { get; set; }

    /// <inheritdoc/>
    public decimal High { get; set; }

    /// <inheritdoc/>
    public decimal Low { get; set; }

    /// <inheritdoc/>
    public decimal Close { get; set; }

    /// <inheritdoc/>
    public decimal Volume { get; set; }
}

// internal use only, double variant
internal record class QuoteD
{
    internal DateTime Date { get; set; }
    internal double Open { get; set; }
    internal double High { get; set; }
    internal double Low { get; set; }
    internal double Close { get; set; }
    internal double Volume { get; set; }
}
