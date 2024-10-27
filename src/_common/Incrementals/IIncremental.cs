namespace Skender.Stock.Indicators;

public interface IAddReusable
{
    /// <summary>
    /// Converts an incremental value into
    /// the next incremental indicator value
    /// and added it to the list.
    /// </summary>
    /// <param name="timestamp">Date context</param>
    /// <param name="value">Next value</param>
    void Add(DateTime timestamp, double value);

    /// <summary>
    /// Converts an incremental reusable value into
    /// the next incremental indicator value
    /// and added it to the list.
    /// </summary>
    /// <param name="value">Next value</param>
    void Add(IReusable value);

    /// <summary>
    /// Converts batch of reusable values into
    /// the next incremental indicator values
    /// and added them to the list.
    /// </summary>
    /// <param name="values">
    /// Chronologically ordered batch of IReuslable info
    /// </param>
    void Add(IReadOnlyList<IReusable> values);
}

public interface IAddQuote
{
    /// <summary>
    /// Converts an incremental quote into
    /// the next incremental indicator value
    /// and added it to the list.
    /// </summary>
    /// <param name="quote">Next quote value</param>
    void Add(IQuote quote);

    /// <summary>
    /// Converts batch of quotes into
    /// the next incremental indicator values
    /// and added them to the list.
    /// </summary>
    /// <param name="quotes">
    /// Chronologically ordered batch of quotes
    /// </param>
    void Add(IReadOnlyList<IQuote> quotes);
}
