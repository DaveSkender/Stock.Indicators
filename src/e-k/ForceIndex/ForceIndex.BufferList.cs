namespace Skender.Stock.Indicators;

/// <summary>
/// Force Index from incremental quote values.
/// </summary>
public class ForceIndexList : BufferList<ForceIndexResult>, IIncrementFromQuote, IForceIndex
{
    private readonly Queue<double> _rawFiBuffer;
    private double _sumRawFi;
    private double? _previousFi;
    private double? _previousClose;
    private readonly double _k;

    /// <summary>
    /// Initializes a new instance of the <see cref="ForceIndexList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation. Default is 2.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="lookbackPeriods"/> is invalid.</exception>
    public ForceIndexList(
        int lookbackPeriods = 2
    )
    {
        ForceIndex.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        _rawFiBuffer = new Queue<double>(lookbackPeriods);
        _sumRawFi = 0;
        _previousFi = null;
        _previousClose = null;
        _k = 2d / (lookbackPeriods + 1);

        Name = $"FORCEINDEX({lookbackPeriods})";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForceIndexList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public ForceIndexList(
        int lookbackPeriods,
        IReadOnlyList<IQuote> quotes
    )
        : this(lookbackPeriods) => Add(quotes);

    /// <inheritdoc />
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        // skip first period
        if (Count == 0)
        {
            _previousClose = (double)quote.Close;
            AddInternal(new ForceIndexResult(quote.Timestamp));
            return;
        }

        double? fi = null;

        // calculate raw Force Index
        double? rawFi = (double)quote.Volume * ((double)quote.Close - _previousClose);

        if (rawFi.HasValue)
        {
            // update buffer
            double? dequeuedValue = _rawFiBuffer.UpdateWithDequeue(LookbackPeriods, rawFi.Value);

            if (_rawFiBuffer.Count == LookbackPeriods && dequeuedValue.HasValue)
            {
                _sumRawFi = _sumRawFi - dequeuedValue.Value + rawFi.Value;
            }
            else
            {
                _sumRawFi += rawFi.Value;
            }

            // calculate EMA
            if (Count >= LookbackPeriods)
            {
                if (!_previousFi.HasValue)
                {
                    // initialization period - first EMA value
                    fi = _sumRawFi / LookbackPeriods;
                }
                else
                {
                    fi = _previousFi + (_k * (rawFi - _previousFi));
                }
            }
        }

        _previousFi = fi;
        _previousClose = (double)quote.Close;

        AddInternal(new ForceIndexResult(
            Timestamp: quote.Timestamp,
            ForceIndex: fi));
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
        _rawFiBuffer.Clear();
        _sumRawFi = 0;
        _previousFi = null;
        _previousClose = null;
    }
}

public static partial class ForceIndex
{
    /// <summary>
    /// Creates a buffer list for Force Index calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    public static ForceIndexList ToForceIndexList(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 2)
        => new(lookbackPeriods) { quotes };
}
