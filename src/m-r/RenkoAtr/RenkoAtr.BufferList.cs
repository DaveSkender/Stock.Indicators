namespace Skender.Stock.Indicators;

/// <summary>
/// Renko Chart (ATR) from incremental quote values.
/// </summary>
public class RenkoAtrList : BufferList<RenkoResult>, IIncrementFromQuote, IRenkoAtr
{
    private readonly List<IQuote> _quoteBuffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="RenkoAtrList"/> class.
    /// </summary>
    /// <param name="atrPeriods">The number of periods for calculating ATR.</param>
    /// <param name="endType">The price candle end type to use as the brick threshold.</param>
    public RenkoAtrList(
        int atrPeriods = 14,
        EndType endType = EndType.Close)
    {
        Atr.Validate(atrPeriods);
        AtrPeriods = atrPeriods;
        EndType = endType;
        _quoteBuffer = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RenkoAtrList"/> class with initial quotes.
    /// </summary>
    /// <param name="atrPeriods">The number of periods for calculating ATR.</param>
    /// <param name="endType">The price candle end type to use as the brick threshold.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public RenkoAtrList(
        int atrPeriods,
        EndType endType,
        IReadOnlyList<IQuote> quotes)
        : this(atrPeriods, endType) => Add(quotes);

    /// <inheritdoc/>
    public int AtrPeriods { get; init; }

    /// <inheritdoc/>
    public EndType EndType { get; init; }




    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        // Add quote to buffer
        _quoteBuffer.Add(quote);

        // Regenerate all results with current set of quotes
        RegenerateResults();
    }

    /// <inheritdoc />
    public void Add(IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);

        // Add all quotes to buffer
        _quoteBuffer.AddRange(quotes);

        // Regenerate all results with current set of quotes
        RegenerateResults();
    }

    private void RegenerateResults()
    {
        // Clear existing results
        while (Count > 0)
        {
            RemoveAt(Count - 1);
        }

        if (_quoteBuffer.Count == 0)
        {
            return;
        }

        // Calculate ATR for all buffered quotes
        IReadOnlyList<AtrResult> atrResults = _quoteBuffer.ToAtr(AtrPeriods);

        // Get the last ATR value as brick size
        AtrResult? lastAtr = atrResults[^1];
        decimal brickSize = (decimal?)lastAtr?.Atr ?? 0;

        if (brickSize > 0)
        {
            // Generate Renko results with the current brick size
            IReadOnlyList<RenkoResult> renkoResults = _quoteBuffer.ToRenko(brickSize, EndType);

            // Add all results
            foreach (RenkoResult result in renkoResults)
            {
                AddInternal(result);
            }
        }
    }

    /// <inheritdoc />
    public override void Clear()
    {
        base.Clear();
        _quoteBuffer.Clear();
    }
}

public static partial class RenkoAtr
{
    /// <summary>
    /// Creates a buffer list for Renko Chart (ATR) calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="atrPeriods">The number of periods for calculating ATR.</param>
    /// <param name="endType">The price candle end type to use as the brick threshold.</param>
    /// <returns>A buffer list for Renko Chart (ATR) calculations.</returns>
    public static RenkoAtrList ToRenkoAtrList(
        this IReadOnlyList<IQuote> quotes,
        int atrPeriods = 14,
        EndType endType = EndType.Close)
        => new(atrPeriods, endType) { quotes };
}
