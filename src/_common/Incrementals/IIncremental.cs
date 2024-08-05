namespace Skender.Stock.Indicators;

public interface IIncrementalPrice<TQuote>
    where TQuote : IQuote
{
    /// <summary>
    /// Converts incremental price into
    /// the next incremental indicator value
    /// and added it to the list.
    /// </summary>
    /// <param name="timestamp">Date context</param>
    /// <param name="price">Next price value</param>
    void Add(DateTime timestamp, double price);

    /// <summary>
    /// Converts incremental quotes into
    /// the next incremental indicator value
    /// and added it to the list.
    /// </summary>
    /// <param name="quote">Next quote value</param>
    void Add(TQuote quote);

    // TODO: convert ToStreamHub();
}

public interface IIncrementalQuote<TQuote>
    where TQuote : IQuote
{
    /// <inheritdoc cref="IIncrementalPrice{TQuote}.Add(TQuote)"/>
    void Add(TQuote quote);
}

/// <remarks>
/// This produces the same results as the equivalent
/// time-series indicator, but is optimized for array-based operations.
/// Since it does not retain a date context,
/// all new values provided to the <see cref="Add(double)"/>
/// method are expected to be in chronological order.
/// </remarks>
public interface IIncrementalValue
{
    /// <inheritdoc cref="IIncrementalPrice{TQuote}.Add(DateTime, double)"/>
    void Add(double price);
}
