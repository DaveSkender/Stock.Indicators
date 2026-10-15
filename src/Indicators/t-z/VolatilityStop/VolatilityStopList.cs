namespace FacioQuo.Stock.Indicators;

/// <summary>
/// Volatility Stop from incremental bars.
/// </summary>
public class VolatilityStopList : BufferList<VolatilityStopResult>, IIncrementFromBar
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

    // rows seen since construction or Clear; Count shrinks when the list prunes
    private int _rowsSeen;

    /// <summary>
    /// Initializes a new instance of the <see cref="VolatilityStopList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">Multiplier for the Average True Range.</param>
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
    /// Initializes a new instance of the <see cref="VolatilityStopList"/> class with initial bars.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">Multiplier for the Average True Range.</param>
    /// <param name="bars">Aggregate OHLCV price bars, time sorted.</param>
    public VolatilityStopList(int lookbackPeriods, double multiplier, IReadOnlyList<IBar> bars)
        : this(lookbackPeriods, multiplier) => Add(bars);

    /// <inheritdoc />
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public double Multiplier { get; init; }

    /// <inheritdoc />
    public void Add(IBar bar)
    {
        ArgumentNullException.ThrowIfNull(bar);

        DateTime timestamp = bar.Timestamp;
        double close = (double)bar.Close;

        // Add to ATR list
        _atrList.Add(bar);

        // Track close prices during initialization
        if (_rowsSeen < _lookbackPeriods)
        {
            _closePrices.Add(close);
        }

        // During initialization period (first lookbackPeriods bars)
        if (_rowsSeen++ < _lookbackPeriods)
        {
            // On the last initialization period, determine trend direction
            if (_rowsSeen == _lookbackPeriods)
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

        // Calculate SAR using the previous bar's ATR
        AtrResult atr = _atrList[^2];
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
    public void Add(IReadOnlyList<IBar> bars)
    {
        ArgumentNullException.ThrowIfNull(bars);

        for (int i = 0; i < bars.Count; i++)
        {
            Add(bars[i]);
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
        _rowsSeen = 0;
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
        // rows pruned off the front shift the index of the first row to nullify
        for (int i = Math.Max(0, _lookbackPeriods - (_rowsSeen - Count)); i < Count; i++)
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
    /// <param name="bars">Aggregate OHLCV price bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">Multiplier for calculation</param>
    public static VolatilityStopList ToVolatilityStopList(
        this IReadOnlyList<IBar> bars,
        int lookbackPeriods = 7,
        double multiplier = 3)
        => new(lookbackPeriods, multiplier) { bars };
}
