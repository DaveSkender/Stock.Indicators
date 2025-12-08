namespace Skender.Stock.Indicators;

/// <summary>
/// Williams %R from incremental quote values.
/// </summary>
public class WilliamsRList : BufferList<WilliamsResult>, IIncrementFromQuote, IWilliamsR
{
    private readonly StochList _stochList;

    /// <summary>
    /// Initializes a new instance of the <see cref="WilliamsRList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    public WilliamsRList(
        int lookbackPeriods = 14)
    {
        WilliamsR.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        // Williams %R is Fast Stochastic (K) - 100
        // Fast Stochastic parameters: lookback, signal=1, smooth=1
        _stochList = new StochList(lookbackPeriods, 1, 1, 3, 2, MaType.SMA);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WilliamsRList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public WilliamsRList(
        int lookbackPeriods,
        IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods) => Add(quotes);

    /// <summary>
    /// Gets the lookback periods for Williams %R calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);
        _stochList.Add(quote);

        // Convert Stochastic result to Williams %R
        StochResult stochResult = _stochList[_stochList.Count - 1];
        WilliamsResult williamsResult = new(
            Timestamp: stochResult.Timestamp,
            WilliamsR: stochResult.Oscillator - 100d);

        AddInternal(williamsResult);
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
        _stochList.Clear();
    }
}
