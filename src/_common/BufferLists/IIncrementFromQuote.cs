namespace Skender.Stock.Indicators;

/// <summary>
/// Interface for incrementing with OHLCV <see cref="IQuote"/> values.
/// </summary>
/// <remarks>
/// This patterns supports indicators that can only be calculated from OHLCV quotes.
/// Examples: TR, RSI, moving averages, and others.
/// </remarks>
public interface IIncrementFromQuote
{
    /// <summary>
    /// Apply new quote to increment indicator list values.
    /// </summary>
    /// <remarks>
    /// Quotes must be supplied in chronological order. This method assumes
    /// <paramref name="quote"/> is newer than every previously added value;
    /// it does not detect or correct out-of-order, duplicate, or revised
    /// timestamps. Use a <c>StreamHub</c> when input can arrive out of order.
    /// </remarks>
    /// <param name="quote">Next quote value, newer than the last added.</param>
    void Add(IQuote quote);

    /// <summary>
    /// Apply batch of quotes increment many indicator list values.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, in chronological order.</param>
    void Add(IReadOnlyList<IQuote> quotes);
}
