namespace Skender.Stock.Indicators;

/// <summary>
/// Choppiness Index (CHOP) from incremental quotes.
/// </summary>
public class ChopList : BufferList<ChopResult>, IBufferList, IChop
{
    private readonly Queue<double> _trueHighBuffer;
    private readonly Queue<double> _trueLowBuffer;
    private readonly Queue<double> _trueRangeBuffer;
    private double _previousClose;
    private bool _isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChopList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to use for the lookback window.</param>
    public ChopList(int lookbackPeriods = 14)
    {
        Chop.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        _trueHighBuffer = new Queue<double>(lookbackPeriods);
        _trueLowBuffer = new Queue<double>(lookbackPeriods);
        _trueRangeBuffer = new Queue<double>(lookbackPeriods);
        _isInitialized = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChopList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to use for the lookback window.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public ChopList(int lookbackPeriods, IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods)
        => Add(quotes);

    /// <summary>
    /// Gets the number of periods to use for the lookback window.
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

        double? chop = null;

        if (!_isInitialized)
        {
            _previousClose = close;
            _isInitialized = true;
        }
        else
        {
            // Calculate true high, true low, and true range
            double trueHigh = Math.Max(high, _previousClose);
            double trueLow = Math.Min(low, _previousClose);
            double trueRange = trueHigh - trueLow;

            // Update buffers
            _trueHighBuffer.Update(LookbackPeriods, trueHigh);
            _trueLowBuffer.Update(LookbackPeriods, trueLow);
            _trueRangeBuffer.Update(LookbackPeriods, trueRange);

            // Calculate CHOP when we have enough data
            if (_trueRangeBuffer.Count == LookbackPeriods)
            {
                double sumTrueRange = _trueRangeBuffer.Sum();
                double maxTrueHigh = _trueHighBuffer.Max();
                double minTrueLow = _trueLowBuffer.Min();
                double range = maxTrueHigh - minTrueLow;

                if (range != 0)
                {
                    chop = 100 * (Math.Log(sumTrueRange / range) / Math.Log(LookbackPeriods));
                }
            }

            _previousClose = close;
        }

        AddInternal(new ChopResult(timestamp, chop));
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
        ClearInternal();
        _trueHighBuffer.Clear();
        _trueLowBuffer.Clear();
        _trueRangeBuffer.Clear();
        _previousClose = 0;
        _isInitialized = false;
    }
}

public static partial class Chop
{
    /// <summary>
    /// Creates a buffer list for Choppiness Index (CHOP) calculations.
    /// </summary>
    public static ChopList ToChopList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 14)
        where TQuote : IQuote
        => new(lookbackPeriods) { (IReadOnlyList<IQuote>)quotes };
}
