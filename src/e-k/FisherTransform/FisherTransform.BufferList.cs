namespace Skender.Stock.Indicators;

/// <summary>
/// Fisher Transform from incremental reusable values.
/// </summary>
public class FisherTransformList : BufferList<FisherTransformResult>, IBufferReusable
{
    private readonly Queue<double> _priceBuffer;
    private double _previousXv;

    /// <summary>
    /// Initializes a new instance of the <see cref="FisherTransformList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation. Default is 10.</param>
    public FisherTransformList(
        int lookbackPeriods = 10
    )
    {
        FisherTransform.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        _priceBuffer = new Queue<double>(lookbackPeriods);
        _previousXv = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FisherTransformList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public FisherTransformList(
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
    public void Add(DateTime timestamp, double value)
    {
        // add current value to buffer first
        _priceBuffer.Update(LookbackPeriods, value);

        // calculate min/max in lookback period (all values in buffer)
        double minPrice = value;
        double maxPrice = value;

        foreach (double price in _priceBuffer)
        {
            if (price < minPrice) minPrice = price;
            if (price > maxPrice) maxPrice = price;
        }

        double? fisher;
        double? trigger = null;

        if (Count > 0)
        {
            // calculate value transform
            double xv = maxPrice - minPrice != 0
                ? (0.33 * 2 * (((value - minPrice) / (maxPrice - minPrice)) - 0.5))
                      + (0.67 * _previousXv)
                : 0;

            // limit xv to prevent log issues
            xv = xv > 0.99 ? 0.999 : xv;
            xv = xv < -0.99 ? -0.999 : xv;

            // calculate Fisher Transform
            fisher = ((0.5 * Math.Log((1 + xv) / (1 - xv)))
                  + (0.5 * this[^1].Fisher)).NaN2Null();

            trigger = this[^1].Fisher;
            _previousXv = xv;
        }
        else
        {
            _previousXv = 0;
            fisher = 0;
        }

        AddInternal(new FisherTransformResult(
            Timestamp: timestamp,
            Fisher: fisher,
            Trigger: trigger));
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
        // FisherTransform prefers HL2 for IQuote
        double hl2 = ((double)quote.High + (double)quote.Low) / 2;
        Add(quote.Timestamp, hl2);
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
        _priceBuffer.Clear();
        _previousXv = 0;
    }
}

public static partial class FisherTransform
{
    /// <summary>
    /// Creates a buffer list for Fisher Transform calculations.
    /// </summary>
    public static FisherTransformList ToFisherTransformList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 10)
        where TQuote : IQuote
        => new(lookbackPeriods) { (IReadOnlyList<IQuote>)quotes };
}
