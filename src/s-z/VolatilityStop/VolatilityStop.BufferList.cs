namespace Skender.Stock.Indicators;

/// <summary>
/// Volatility Stop from incremental quotes.
/// </summary>
public class VolatilityStopList : BufferList<VolatilityStopResult>, IIncrementFromQuote
{
    private readonly int _lookbackPeriods;
    private readonly double _multiplier;
    private readonly AtrList _atrList;
    /// <summary>
    /// Track close prices for initialization
    /// </summary>
    private readonly List<double> _closePrices;
    /// <summary>
    /// significant close
    /// </summary>
    private double _sic;
    private bool _isLong;
    private bool _firstStopFound;

    /// <summary>
    /// Initializes a new instance of the <see cref="VolatilityStopList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">The multiplier for the Average True Range.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="multiplier"/> is invalid.</exception>
    public VolatilityStopList(int lookbackPeriods = 7, double multiplier = 3)
    {
        VolatilityStop.Validate(lookbackPeriods, multiplier);
        _lookbackPeriods = lookbackPeriods;
        _multiplier = multiplier;
        LookbackPeriods = lookbackPeriods;
        Multiplier = multiplier;
        _atrList = new AtrList(lookbackPeriods);
        _closePrices = [];
        _firstStopFound = false;

        Name = $"VOLATILITYSTOP({lookbackPeriods}, {multiplier})";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VolatilityStopList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">The multiplier for the Average True Range.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public VolatilityStopList(int lookbackPeriods, double multiplier, IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods, multiplier) => Add(quotes);

    /// <inheritdoc />
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public double Multiplier { get; init; }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        DateTime timestamp = quote.Timestamp;
        double close = (double)quote.Close;

        // Add to ATR list
        _atrList.Add(quote);

        // Track close prices during initialization
        _closePrices.Add(close);

        // During initialization period (first lookbackPeriods quotes)
        if (Count < _lookbackPeriods)
        {
            // On the last initialization period, determine trend direction
            if (Count == _lookbackPeriods - 1)
            {
                _sic = _closePrices[0];
                _isLong = close > _sic;

                // Update sic for all initialization periods based on determined trend
                for (int j = 0; j < _closePrices.Count; j++)
                {
                    _sic = _isLong ? Math.Max(_sic, _closePrices[j]) : Math.Min(_sic, _closePrices[j]);
                }
            }

            // Add null result for initialization period
            AddInternal(new VolatilityStopResult(timestamp));
            return;
        }

        // Calculate SAR using previous ATR (at Count - 1, which is index i - 1 in series)
        AtrResult atr = _atrList[Count - 1];
        double? arc = atr.Atr * _multiplier;
        double? sar = _isLong ? _sic - arc : _sic + arc;

        // Determine bands
        double? lowerBand = null;
        double? upperBand = null;

        if (_isLong)
        {
            lowerBand = sar;
        }
        else
        {
            upperBand = sar;
        }

        // Evaluate stop and reverse
        bool? isStop;

        if ((_isLong && close < sar) || (!_isLong && close > sar))
        {
            isStop = true;
            _sic = close;
            _isLong = !_isLong;
        }
        else
        {
            isStop = false;
            // Update significant close
            _sic = _isLong ? Math.Max(_sic, close) : Math.Min(_sic, close);
        }

        VolatilityStopResult result = new(
            Timestamp: timestamp,
            Sar: sar,
            IsStop: isStop,
            UpperBand: upperBand,
            LowerBand: lowerBand);

        AddInternal(result);

        // If this was the first stop, nullify all previous results (including this one)
        if (isStop == true && !_firstStopFound)
        {
            _firstStopFound = true;
            NullifyResultsBeforeFirstStop();
        }
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
        _atrList.Clear();
        _closePrices.Clear();
        _sic = 0;
        _isLong = false;
        _firstStopFound = false;
    }
    /// <summary>
    /// Overrides list pruning to synchronize the nested ATR list.
    /// </summary>
    protected override void PruneList()
    {
        // Synchronize the nested ATR list's MaxListSize with the outer list
        // Keep enough ATR history to support calculations
        int minAtrSize = _lookbackPeriods + 1;
        _atrList.MaxListSize = Math.Max(minAtrSize, MaxListSize + 1);

        // Call base implementation to prune the outer result list
        base.PruneList();
    }

    private void NullifyResultsBeforeFirstStop()
    {
        // Update all results from lookbackPeriods to current (inclusive of the stop we just added)
        for (int i = _lookbackPeriods; i < Count; i++)
        {
            VolatilityStopResult existing = this[i];
            VolatilityStopResult updated = existing with {
                Sar = null,
                UpperBand = null,
                LowerBand = null,
                IsStop = null
            };
            UpdateInternal(i, updated);
        }
    }
}

public static partial class VolatilityStop
{
    /// <summary>
    /// Creates a buffer list for Volatility Stop calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">Multiplier for calculation</param>
    public static VolatilityStopList ToVolatilityStopList(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 7,
        double multiplier = 3)
        => new(lookbackPeriods, multiplier) { quotes };
}
