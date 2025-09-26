namespace Skender.Stock.Indicators;

/// <summary>
/// Abstract base class for streaming indicators that provides common functionality
/// for incremental update patterns and buffer management.
/// </summary>
/// <typeparam name="TIn">The type of input data.</typeparam>
/// <typeparam name="TOut">The type of output result.</typeparam>
public abstract class StreamingIndicatorBase<TIn, TOut> : IStreamingIndicator<TIn, TOut>
    where TIn : ISeries
    where TOut : ISeries
{
    private readonly List<TOut> _results = [];
    private int _processedCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamingIndicatorBase{TIn, TOut}"/> class.
    /// </summary>
    /// <param name="warmupPeriod">The minimum number of input values required for valid results.</param>
    protected StreamingIndicatorBase(int warmupPeriod)
    {
        if (warmupPeriod < 0)
            throw new ArgumentOutOfRangeException(nameof(warmupPeriod), "Warmup period cannot be negative.");

        WarmupPeriod = warmupPeriod;
    }

    /// <inheritdoc/>
    public bool IsInitialized => _processedCount >= WarmupPeriod;

    /// <inheritdoc/>
    public int WarmupPeriod { get; }

    /// <inheritdoc/>
    public IReadOnlyList<TOut> Results => _results.AsReadOnly();

    /// <summary>
    /// Gets the number of input values processed so far.
    /// </summary>
    protected int ProcessedCount => _processedCount;

    /// <inheritdoc/>
    public TOut? ProcessIncremental(TIn input)
    {
        ArgumentNullException.ThrowIfNull(input);

        _processedCount++;

        // Let derived class perform the actual calculation
        var result = CalculateNext(input);

        // Add result to collection if it's valid
        if (result != null)
        {
            _results.Add(result);
        }

        return result;
    }

    /// <inheritdoc/>
    public IEnumerable<TOut?> ProcessBatch(IEnumerable<TIn> inputs)
    {
        ArgumentNullException.ThrowIfNull(inputs);

        var results = new List<TOut?>();
        
        foreach (var input in inputs)
        {
            var result = ProcessIncremental(input);
            results.Add(result);
        }

        return results;
    }

    /// <inheritdoc/>
    public virtual void Reset()
    {
        _results.Clear();
        _processedCount = 0;
        ResetState();
    }

    /// <inheritdoc/>
    public abstract string GetShortName();

    /// <summary>
    /// Calculates the next result based on the new input data.
    /// This method is called by <see cref="ProcessIncremental(TIn)"/>.
    /// </summary>
    /// <param name="input">The new input data.</param>
    /// <returns>The calculated result, or null if not enough data for valid calculation.</returns>
    protected abstract TOut? CalculateNext(TIn input);

    /// <summary>
    /// Resets any internal state specific to the derived indicator.
    /// This method is called by <see cref="Reset()"/>.
    /// </summary>
    protected virtual void ResetState()
    {
        // Default implementation does nothing
        // Derived classes can override to reset their specific state
    }

    /// <summary>
    /// Creates a new result instance. Derived classes should implement this
    /// to create the appropriate result type.
    /// </summary>
    /// <param name="timestamp">The timestamp for the result.</param>
    /// <returns>A new result instance.</returns>
    protected abstract TOut CreateResult(DateTime timestamp);

    /// <summary>
    /// Gets the timestamp from an input item.
    /// </summary>
    /// <param name="input">The input item.</param>
    /// <returns>The timestamp from the input.</returns>
    protected static DateTime GetInputTimestamp(TIn input) => input.Timestamp;
}