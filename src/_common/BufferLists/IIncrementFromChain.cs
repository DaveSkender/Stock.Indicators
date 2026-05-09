namespace Skender.Stock.Indicators;

/// <summary>
/// Interface for incrementing with <see cref="IReusable"/> values.
/// </summary>
/// <remarks>
/// This patterns supports chainable indicators that can be calculated from single values.
/// Examples: TR, RSI, moving averages, and others.
/// </remarks>
public interface IIncrementFromChain
{
    /// <summary>
    /// Apply new reusable input value to increment indicator list values.
    /// </summary>
    /// <param name="timestamp">Date context.</param>
    /// <param name="value">Next value.</param>
    void Add(DateTime timestamp, double value);

    /// <summary>
    /// Apply new input value to increment indicator list values.
    /// </summary>
    /// <param name="value">Next reusable value.</param>
    void Add(IReusable value);

    /// <summary>
    /// Apply batch of reusable input values to increment many indicator list values.
    /// </summary>
    /// <param name="values">A chronologically ordered batch of <see cref="IReusable"/> values.</param>
    void Add(IReadOnlyList<IReusable> values);
}
