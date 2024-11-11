namespace Skender.Stock.Indicators;

/// <summary>
/// Interface for adding reusable incremental values to a list.
/// </summary>
public interface IAddReusable
{
    /// <summary>
    /// Converts an incremental value into the next incremental indicator value and adds it to the list.
    /// </summary>
    /// <param name="timestamp">The date context.</param>
    /// <param name="value">The next value.</param>
    void Add(DateTime timestamp, double value);

    /// <summary>
    /// Converts an incremental reusable value into the next incremental indicator value and adds it to the list.
    /// </summary>
    /// <param name="value">The next reusable value.</param>
    void Add(IReusable value);

    /// <summary>
    /// Converts a batch of reusable values into the next incremental indicator values and adds them to the list.
    /// </summary>
    /// <param name="values">A chronologically ordered batch of <see cref="IReusable"/> values.</param>
    void Add(IReadOnlyList<IReusable> values);
}

/// <summary>
/// Interface for adding incremental quotes to a list.
/// </summary>
public interface IAddQuote
{
    /// <summary>
    /// Converts an incremental quote into the next incremental indicator value and adds it to the list.
    /// </summary>
    /// <param name="quote">The next quote value.</param>
    void Add(IQuote quote);

    /// <summary>
    /// Converts a batch of quotes into the next incremental indicator values and adds them to the list.
    /// </summary>
    /// <param name="quotes">A chronologically ordered batch of quotes.</param>
    void Add(IReadOnlyList<IQuote> quotes);
}
