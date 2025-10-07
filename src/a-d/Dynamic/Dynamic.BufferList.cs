namespace Skender.Stock.Indicators;

/// <summary>
/// McGinley Dynamic from incremental reusable values.
/// </summary>
public class DynamicList : BufferList<DynamicResult>, IBufferReusable
{
    private double? _previousDynamic;
    private double _previousValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="kFactor">The smoothing factor for the calculation.</param>
    public DynamicList(
        int lookbackPeriods,
        double kFactor = 0.6
    )
    {
        MgDynamic.Validate(lookbackPeriods, kFactor);
        LookbackPeriods = lookbackPeriods;
        KFactor = kFactor;

        _previousDynamic = null;
        _previousValue = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="kFactor">The smoothing factor for the calculation.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public DynamicList(
        int lookbackPeriods,
        double kFactor,
        IReadOnlyList<IQuote> quotes
    )
        : this(lookbackPeriods, kFactor)
        => Add(quotes);

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <summary>
    /// Gets the smoothing factor for the calculation.
    /// </summary>
    public double KFactor { get; init; }


    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        // skip first period
        if (Count == 0)
        {
            _previousDynamic = null;
            _previousValue = value;
            AddInternal(new DynamicResult(timestamp, null));
            return;
        }

        // calculate dynamic
        double prevDyn = _previousDynamic ?? _previousValue;
        double? dyn = MgDynamic.Increment(
            LookbackPeriods,
            KFactor,
            newVal: value,
            prevDyn: prevDyn
        ).NaN2Null();

        _previousDynamic = dyn;
        _previousValue = value;
        AddInternal(new DynamicResult(timestamp, dyn));
    }

    /// <inheritdoc />
    public void Add(IReusable value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Add(value.Timestamp, value.Value);
    }

    /// <inheritdoc />
    public void Add(IReadOnlyList<IReusable> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        for (int i = 0; i < values.Count; i++)
        {
            Add(values[i].Timestamp, values[i].Value);
        }
    }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);
        Add(quote.Timestamp, quote.Value);
    }

    /// <inheritdoc />
    public void Add(IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);

        for (int i = 0; i < quotes.Count; i++)
        {
            Add(quotes[i].Timestamp, quotes[i].Value);
        }
    }

    /// <inheritdoc />
    public override void Clear()
    {
        ClearInternal();
        _previousDynamic = null;
        _previousValue = 0;
    }
}

public static partial class MgDynamic
{
    /// <summary>
    /// Creates a buffer list for McGinley Dynamic calculations.
    /// </summary>
    public static DynamicList ToDynamicList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods,
        double kFactor = 0.6)
        where TQuote : IQuote
        => new(lookbackPeriods, kFactor) { (IReadOnlyList<IQuote>)quotes };
}
