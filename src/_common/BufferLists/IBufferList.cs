namespace Skender.Stock.Indicators;

/// <summary>
/// Base interface for buffered indicator lists.
/// </summary>
public interface IBufferList
{
    /// <summary>
    /// Apply new quote to increment indicator list values.
    /// </summary>
    /// <param name="quote">The next quote value.</param>
    void Add(IQuote quote);

    /// <summary>
    /// Apply batch of quotes increment many indicator list values.
    /// </summary>
    /// <param name="quotes">A chronologically ordered batch of quotes.</param>
    void Add(IReadOnlyList<IQuote> quotes);

    /// <summary>
    /// Clears the indicator list values and input buffers, so the instance can be reused.
    /// </summary>
    void Clear();
}

/// <summary>
/// Interface for adding input values to a buffered list.
/// </summary>
public interface IBufferReusable : IBufferList
{
    /// <summary>
    /// Apply new reusable input value to increment indicator list values.
    /// </summary>
    /// <param name="timestamp">The date context.</param>
    /// <param name="value">The next value.</param>
    void Add(DateTime timestamp, double value);

    /// <summary>
    /// Apply new input value to increment indicator list values.
    /// </summary>
    /// <param name="value">The next reusable value.</param>
    void Add(IReusable value);

    /// <summary>
    /// Apply batch of reusable input values to increment many indicator list values.
    /// </summary>
    /// <param name="values">A chronologically ordered batch of <see cref="IReusable"/> values.</param>
    void Add(IReadOnlyList<IReusable> values);
}
