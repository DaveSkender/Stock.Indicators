namespace Skender.Stock.Indicators;

/// <summary>
/// Average True Range (ATR) from incremental quotes.
/// </summary>
public class AtrList : List<AtrResult>, IAtr, IBufferList
{
    private readonly int _lookbackPeriods;
    private double _previousClose;
    private double? _previousAtr;
    private double _sumTr;
    private bool _isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="AtrList"/> class.
    /// </summary>
    public AtrList(int lookbackPeriods)
    {
        Atr.Validate(lookbackPeriods);
        _lookbackPeriods = lookbackPeriods;
        LookbackPeriods = lookbackPeriods;
        _isInitialized = false;
        _sumTr = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AtrList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public AtrList(int lookbackPeriods, IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods)
    {
        Add(quotes);
    }

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
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

            base.Add(new AtrResult(timestamp, null, null, null));
            return;
        }

        // Calculate True Range
        double tr = Tr.Increment(high, low, _previousClose);

        double atr;
        double? atrp;

        if (Count > _lookbackPeriods)
        {
            // Use ATR incremental calculation for steady state
            atr = ((_previousAtr!.Value * (_lookbackPeriods - 1)) + tr) / _lookbackPeriods;
            atrp = close == 0 ? null : atr / close * 100;
            _previousAtr = atr;
        }
        else if (Count == _lookbackPeriods)
        {
            // Initialize ATR after accumulating enough TR values
            _sumTr += tr;
            atr = _sumTr / _lookbackPeriods;
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

        base.Add(result);
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
    public new void Clear()
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
    public static AtrList ToAtrList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
        => new(lookbackPeriods) { (IReadOnlyList<IQuote>)quotes };
}
