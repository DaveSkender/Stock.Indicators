namespace Skender.Stock.Indicators;

/// <summary>
/// Base interface for buffered indicator lists that support resetting their state.
/// </summary>
public interface IBufferList
{
    /// <summary>
    /// Apply new quote for calculating incremental indicator values and storing calculated results in the list.
    /// </summary>
    /// <param name="quote">The next quote value.</param>
    void Add(IQuote quote);

    /// <summary>
    /// Apply batch of quotes for calculating incremental indicator values and storing calculated results in the list.
    /// </summary>
    /// <param name="quotes">A chronologically ordered batch of quotes.</param>
    void Add(IReadOnlyList<IQuote> quotes);

    /// <summary>
    /// Clears the indicator results and resets any internal buffers so the instance can be reused.
    /// </summary>
    void Clear();
}

/// <summary>
/// Interface for adding input values to a buffered list.
/// </summary>
public interface IBufferReusable : IBufferList
{
    /// <summary>
    /// Apply new input value for calculating incremental indicator values and storing calculated results in the list.
    /// </summary>
    /// <param name="timestamp">The date context.</param>
    /// <param name="value">The next value.</param>
    void Add(DateTime timestamp, double value);

    /// <summary>
    /// Apply new reusable input value for calculating incremental indicator values and storing calculated results in the list.
    /// </summary>
    /// <param name="value">The next reusable value.</param>
    void Add(IReusable value);

    /// <summary>
    /// Apply batch of reusable input values for calculating incremental indicator values and storing calculated results in the list.
    /// </summary>
    /// <param name="values">A chronologically ordered batch of <see cref="IReusable"/> values.</param>
    void Add(IReadOnlyList<IReusable> values);
}
