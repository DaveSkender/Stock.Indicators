namespace Skender.Stock.Indicators;

/// <summary>
/// Average True Range (ATR) from incremental quotes.
/// </summary>
public class AtrList : BufferList<AtrResult>, IIncrementFromQuote, IAtr
{
    private readonly int _lookbackPeriods;
    private double _previousClose;
    private double? _previousAtr;
    private double _sumTr;
    private bool _isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="AtrList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="lookbackPeriods"/> is invalid.</exception>
    public AtrList(int lookbackPeriods)
    {
        Atr.Validate(lookbackPeriods);
        _lookbackPeriods = lookbackPeriods;
        LookbackPeriods = lookbackPeriods;
        _isInitialized = false;
        _sumTr = 0;

        Name = $"ATR({lookbackPeriods})";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AtrList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="lookbackPeriods"/> is invalid.</exception>
    public AtrList(int lookbackPeriods, IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods) => Add(quotes);

    /// <inheritdoc />
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        DateTime timestamp = quote.Timestamp;
        double high = (double)quote.High;
        double low = (double)quote.Low;
        double close = (double)quote.Close;

        // Handle first period - no ATR calculation possible
        if (!_isInitialized)
        {
            _previousClose = close;
            _isInitialized = true;

            AddInternal(new AtrResult(timestamp, null, null, null));
            return;
        }

        // Calculate True Range
        double tr = Tr.Increment(high, low, _previousClose);

        double atr;
        double? atrp;

        if (Count >= _lookbackPeriods)
        {
            if (_previousAtr is null)
            {
                // Initialize ATR after accumulating enough TR values
                _sumTr += tr;
                atr = _sumTr / _lookbackPeriods;
            }
            else
            {
                // Use ATR incremental calculation for steady state
                atr = ((_previousAtr.Value * (_lookbackPeriods - 1)) + tr) / _lookbackPeriods;
            }

            atrp = close == 0 ? null : atr / close * 100;
            _previousAtr = atr;
        }
        else
        {
            // Accumulate TR for initialization
            _sumTr += tr;
            atr = double.NaN;
            atrp = null;
        }

        AtrResult result = new(
            timestamp,
            tr.NaN2Null(),
            atr.NaN2Null(),
            atrp);

        AddInternal(result);
        _previousClose = close;
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
        _previousClose = 0;
        _previousAtr = null;
        _sumTr = 0;
        _isInitialized = false;
    }
}

public static partial class Atr
{
    /// <summary>
    /// Creates a buffer list for Average True Range calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    public static AtrList ToAtrList(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
        => new(lookbackPeriods) { quotes };
}
