namespace Skender.Stock.Indicators;

/// <summary>
/// Chaikin Oscillator from incremental quotes.
/// </summary>
public class ChaikinOscList : BufferList<ChaikinOscResult>, IIncrementFromQuote, IChaikinOsc
{
    private readonly AdlList _adlList;
    private readonly EmaList _fastEmaList;
    private readonly EmaList _slowEmaList;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChaikinOscList"/> class.
    /// </summary>
    /// <param name="fastPeriods">The number of periods to use for the fast EMA.</param>
    /// <param name="slowPeriods">The number of periods to use for the slow EMA.</param>
    public ChaikinOscList(int fastPeriods = 3, int slowPeriods = 10)
    {
        ChaikinOsc.Validate(fastPeriods, slowPeriods);
        FastPeriods = fastPeriods;
        SlowPeriods = slowPeriods;

        _adlList = [];
        _fastEmaList = new EmaList(fastPeriods);
        _slowEmaList = new EmaList(slowPeriods);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChaikinOscList"/> class with initial quotes.
    /// </summary>
    /// <param name="fastPeriods">The number of periods to use for the fast EMA.</param>
    /// <param name="slowPeriods">The number of periods to use for the slow EMA.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public ChaikinOscList(int fastPeriods, int slowPeriods, IReadOnlyList<IQuote> quotes)
        : this(fastPeriods, slowPeriods)
    {
        Add(quotes);
    }

    /// <summary>
    /// Gets the number of periods to use for the fast EMA.
    /// </summary>
    public int FastPeriods { get; init; }

    /// <summary>
    /// Gets the number of periods to use for the slow EMA.
    /// </summary>
    public int SlowPeriods { get; init; }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        DateTime timestamp = quote.Timestamp;

        // Calculate ADL
        _adlList.Add(quote);
        AdlResult adlResult = _adlList[^1];

        // Calculate fast and slow EMAs of ADL
        _fastEmaList.Add(adlResult);
        _slowEmaList.Add(adlResult);

        EmaResult fastEma = _fastEmaList[^1];
        EmaResult slowEma = _slowEmaList[^1];

        // Calculate oscillator
        double? oscillator = fastEma.Ema - slowEma.Ema;

        AddInternal(new ChaikinOscResult(
            timestamp,
            adlResult.MoneyFlowMultiplier,
            adlResult.MoneyFlowVolume,
            adlResult.Adl,
            oscillator,
            fastEma.Ema,
            slowEma.Ema));
    }

    /// <inheritdoc />
    public void Add(IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);

        for (int i = 0; i < quotes.Count; i++)
        {
            Add(quotes[i]);
        }
    }

    /// <inheritdoc />
    public override void Clear()
    {
        base.Clear();
        _adlList.Clear();
        _fastEmaList.Clear();
        _slowEmaList.Clear();
    }
}

public static partial class ChaikinOsc
{
    /// <summary>
    /// Creates a buffer list for Chaikin Oscillator calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="fastPeriods"></param>
    /// <param name="slowPeriods"></param>
    public static ChaikinOscList ToChaikinOscList(
        this IReadOnlyList<IQuote> quotes,
        int fastPeriods = 3,
        int slowPeriods = 10)
        => new(fastPeriods, slowPeriods) { quotes };
}
