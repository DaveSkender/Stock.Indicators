namespace Skender.Stock.Indicators;

public interface IIncrementReusable
{
    /// <summary>
    /// Converts an incremental value into
    /// the next incremental indicator value
    /// and added it to the list.
    /// </summary>
    /// <param name="timestamp">Date context</param>
    /// <param name="value">Next value</param>
    void AddValue(DateTime timestamp, double value);

    /// <summary>
    /// Converts an incremental reusable value into
    /// the next incremental indicator value
    /// and added it to the list.
    /// </summary>
    /// <param name="value">Next value</param>
    void AddValue(IReusable value);

    /// <summary>
    /// Converts batch of reusable values into
    /// the next incremental indicator values
    /// and added them to the list.
    /// </summary>
    /// <param name="values">
    /// Chronologically ordered batch of IReuslable info
    /// </param>
    void AddValues(IReadOnlyList<IReusable> values);
}

public interface IIncrementQuote
{
    /// <summary>
    /// Converts an incremental quote into
    /// the next incremental indicator value
    /// and added it to the list.
    /// </summary>
    /// <param name="quote">Next quote value</param>
    void AddValue(IQuote quote);

    /// <summary>
    /// Converts batch of quotes into
    /// the next incremental indicator values
    /// and added them to the list.
    /// </summary>
    /// <param name="quotes">
    /// Chronologically ordered batch of quotes
    /// </param>
    void AddValues(IReadOnlyList<IQuote> quotes);
}

/// <remarks>
/// This produces the same results as the equivalent
/// time-series indicator, but is optimized for primitive type operations.
/// Since it does not retain a date context,
/// all new values provided to the <see cref="AddValue(double)"/>
/// method are expected to be in chronological order.
/// </remarks>
public interface IIncrementPrimitive
{
    /// <inheritdoc cref="IIncrementReusable.AddValue(DateTime, double)"/>
    void AddValue(double value);

    /// <inheritdoc cref="IIncrementReusable.AddValues(IReadOnlyList{IReusable})"/>
    void AddValues(double[] values);
}
