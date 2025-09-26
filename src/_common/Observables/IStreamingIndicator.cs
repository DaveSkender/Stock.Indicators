namespace Skender.Stock.Indicators;

/// <summary>
/// Interface for streaming indicators that support incremental updates.
/// </summary>
/// <typeparam name="TIn">The type of input data.</typeparam>
/// <typeparam name="TOut">The type of output result.</typeparam>
public interface IStreamingIndicator<in TIn, out TOut>
    where TIn : ISeries
    where TOut : ISeries
{
    /// <summary>
    /// Gets a value indicating whether the indicator has been initialized with enough data
    /// to produce valid results.
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// Gets the minimum number of input values required before the indicator
    /// can produce valid results.
    /// </summary>
    int WarmupPeriod { get; }

    /// <summary>
    /// Gets the current results of the indicator.
    /// </summary>
    IReadOnlyList<TOut> Results { get; }

    /// <summary>
    /// Processes a single incremental update without full recalculation.
    /// </summary>
    /// <param name="input">The new input data to process.</param>
    /// <returns>The updated result, or null if not enough data for valid calculation.</returns>
    TOut? ProcessIncremental(TIn input);

    /// <summary>
    /// Processes a batch of incremental updates efficiently.
    /// </summary>
    /// <param name="inputs">The batch of new input data to process.</param>
    /// <returns>The collection of updated results.</returns>
    IEnumerable<TOut?> ProcessBatch(IEnumerable<TIn> inputs);

    /// <summary>
    /// Resets the indicator to its initial state, clearing all cached data.
    /// </summary>
    void Reset();

    /// <summary>
    /// Gets the indicator's short name for display purposes.
    /// </summary>
    /// <returns>A short string representation of the indicator.</returns>
    string GetShortName();
}