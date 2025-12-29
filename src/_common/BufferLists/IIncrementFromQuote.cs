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
    /// <param name="quote">The next quote value.</param>
    void Add(IQuote quote);

    /// <summary>
    /// Apply batch of quotes increment many indicator list values.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    void Add(IReadOnlyList<IQuote> quotes);
}
