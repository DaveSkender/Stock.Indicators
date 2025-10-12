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
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForceIndexList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public ForceIndexList(
        int lookbackPeriods,
        IReadOnlyList<IQuote> quotes
    )
        : this(lookbackPeriods)
        => Add(quotes);

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
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
            if (Count > LookbackPeriods)
            {
                fi = _previousFi + (_k * (rawFi - _previousFi));
            }
            // initialization period - first EMA value
            else if (Count == LookbackPeriods)
            {
                fi = _sumRawFi / LookbackPeriods;
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
    public static ForceIndexList ToForceIndexList(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 2)
        => new(lookbackPeriods) { quotes };
}
