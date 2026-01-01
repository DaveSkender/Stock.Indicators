namespace Skender.Stock.Indicators;

/// <summary>
/// SuperTrend indicator from incremental quotes.
/// </summary>
public class SuperTrendList : BufferList<SuperTrendResult>, IIncrementFromQuote
{
    private readonly AtrList _atrList;
    private readonly double _multiplier;
    private bool _isBullish = true;
    private double? _upperBand;
    private double? _lowerBand;
    private double? _prevClose;
    private bool _isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="SuperTrendList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">The multiplier for the ATR.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="multiplier"/> is invalid.</exception>
    public SuperTrendList(int lookbackPeriods = 10, double multiplier = 3)
    {
        SuperTrend.Validate(lookbackPeriods, multiplier);
        LookbackPeriods = lookbackPeriods;
        Multiplier = multiplier;

        _multiplier = multiplier;
        _atrList = new AtrList(lookbackPeriods);

        Name = $"SUPERTREND({lookbackPeriods}, {multiplier})";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuperTrendList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">The multiplier for the ATR.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public SuperTrendList(int lookbackPeriods, double multiplier, IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods, multiplier) => Add(quotes);

    /// <inheritdoc />
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public double Multiplier { get; init; }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        // Add to ATR list
        _atrList.Add(quote);

        double? superTrend;
        double? upperOnly;
        double? lowerOnly;

        // Calculate when we have enough data
        if (Count >= LookbackPeriods)
        {
            double? mid = ((double)quote.High + (double)quote.Low) / 2;
            double? atr = _atrList[^1].Atr;

            // potential bands
            double? upperEval = mid + (_multiplier * atr);
            double? lowerEval = mid - (_multiplier * atr);

            // initial values
            if (!_isInitialized)
            {
                _isBullish = (double)quote.Close >= mid;
                _upperBand = upperEval;
                _lowerBand = lowerEval;
                _isInitialized = true;
            }
            else
            {
                // new upper band
                if (upperEval < _upperBand || _prevClose > _upperBand)
                {
                    _upperBand = upperEval;
                }

                // new lower band
                if (lowerEval > _lowerBand || _prevClose < _lowerBand)
                {
                    _lowerBand = lowerEval;
                }
            }

            // supertrend
            if ((double)quote.Close <= (_isBullish ? _lowerBand : _upperBand))
            {
                superTrend = _upperBand;
                upperOnly = _upperBand;
                lowerOnly = null;
                _isBullish = false;
            }
            else
            {
                superTrend = _lowerBand;
                lowerOnly = _lowerBand;
                upperOnly = null;
                _isBullish = true;
            }

            _prevClose = (double)quote.Close;
        }
        else
        {
            superTrend = null;
            upperOnly = null;
            lowerOnly = null;
        }

        AddInternal(new SuperTrendResult(
            Timestamp: quote.Timestamp,
            SuperTrend: (decimal?)superTrend,
            UpperBand: (decimal?)upperOnly,
            LowerBand: (decimal?)lowerOnly));
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
        _isBullish = true;
        _upperBand = null;
        _lowerBand = null;
        _prevClose = null;
        _isInitialized = false;
    }
}

public static partial class SuperTrend
{
    /// <summary>
    /// Creates a buffer list for SuperTrend calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">Multiplier for calculation</param>
    public static SuperTrendList ToSuperTrendList(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 10,
        double multiplier = 3)
        => new(lookbackPeriods, multiplier, quotes);
}
