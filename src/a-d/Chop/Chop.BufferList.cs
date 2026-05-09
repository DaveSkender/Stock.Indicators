namespace Skender.Stock.Indicators;

/// <summary>
/// Choppiness Index (CHOP) from incremental quotes.
/// </summary>
public class ChopList : BufferList<ChopResult>, IIncrementFromQuote, IChop
{
    private readonly Queue<(double TrueHigh, double TrueLow, double TrueRange)> _buffer;
    private double _previousClose;
    private bool _isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChopList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="lookbackPeriods"/> is invalid.</exception>
    public ChopList(int lookbackPeriods = 14)
    {
        Chop.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        _buffer = new Queue<(double, double, double)>(lookbackPeriods);
        _isInitialized = false;

        Name = $"CHOP({lookbackPeriods})";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChopList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="lookbackPeriods"/> is invalid.</exception>
    public ChopList(int lookbackPeriods, IReadOnlyList<IQuote> quotes)
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

            // Update buffer with consolidated tuple
            _buffer.Update(LookbackPeriods, (trueHigh, trueLow, trueRange));

            // Calculate CHOP when we have enough data
            if (_buffer.Count == LookbackPeriods)
            {
                double sumTrueRange = 0;
                double maxTrueHigh = double.MinValue;
                double minTrueLow = double.MaxValue;

                foreach ((double TrueHigh, double TrueLow, double TrueRange) in _buffer)
                {
                    sumTrueRange += TrueRange;
                    if (TrueHigh > maxTrueHigh)
                    {
                        maxTrueHigh = TrueHigh;
                    }

                    if (TrueLow < minTrueLow)
                    {
                        minTrueLow = TrueLow;
                    }
                }

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
        base.Clear();
        _buffer.Clear();
        _previousClose = 0;
        _isInitialized = false;
    }
}

public static partial class Chop
{
    /// <summary>
    /// Creates a buffer list for Choppiness Index (CHOP) calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    public static ChopList ToChopList(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
        => new(lookbackPeriods) { quotes };
}
