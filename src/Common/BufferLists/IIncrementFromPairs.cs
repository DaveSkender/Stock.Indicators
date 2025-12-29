namespace Skender.Stock.Indicators;

/// <summary>
/// Interface for incrementing with <see cref="IReusable"/> pairs.
/// </summary>
/// <remarks>
/// This patterns supports indicators that require paired input values.
/// Examples: BETA, CORRELATION, and others.
/// </remarks>
public interface IIncrementFromPairs
{
    /// <summary>
    /// Apply new reusable input value to increment indicator list values.
    /// </summary>
    /// <param name="timestamp">The date context.</param>
    /// <param name="valueA">The next value (A).</param>
    /// <param name="valueB">The next value (B).</param>
    void Add(DateTime timestamp, double valueA, double valueB);

    /// <summary>
    /// Apply new input value to increment indicator list values.
    /// </summary>
    /// <param name="valueA">The next reusable value (A).</param>
    /// <param name="valueB">The next reusable value (B).</param>
    void Add(IReusable valueA, IReusable valueB);

    /// <summary>
    /// Apply batch of reusable input values to increment many indicator list values.
    /// </summary>
    /// <param name="valuesA">A chronologically ordered batch of <see cref="IReusable"/> values (A).</param>
    /// <param name="valuesB">A chronologically ordered batch of <see cref="IReusable"/> values (B).</param>
    void Add(IReadOnlyList<IReusable> valuesA, IReadOnlyList<IReusable> valuesB);
}
