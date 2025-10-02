namespace Skender.Stock.Indicators;

/// <summary>
/// Volume Weighted Moving Average (VWMA) from incremental quote values.
/// </summary>
public class VwmaList : BufferList<VwmaResult>, IVwma, IBufferList
{
    private readonly Queue<(double price, double volume)> _buffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="VwmaList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public VwmaList(
        int lookbackPeriods
    )
    {
        Vwma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        _buffer = new Queue<(double, double)>(lookbackPeriods);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VwmaList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public VwmaList(
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
        Add(quote.Timestamp, (double)quote.Close, (double)quote.Volume);
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

    /// <summary>
    /// Apply new price and volume values for calculating incremental VWMA values.
    /// </summary>
    /// <param name="timestamp">The date context.</param>
    /// <param name="price">The price value (typically close price).</param>
    /// <param name="volume">The volume value.</param>
    public void Add(DateTime timestamp, double price, double volume)
    {
        // Use BufferUtilities extension method for consistent buffer management
        _buffer.Update(LookbackPeriods, (price, volume));

        // Calculate VWMA when we have enough values by recalculating from buffer
        // This matches the precision of the static series implementation
        double? vwma = null;
        if (_buffer.Count == LookbackPeriods)
        {
            double priceVolumeSum = 0;
            double volumeSum = 0;
            foreach ((double p, double v) in _buffer)
            {
                priceVolumeSum += p * v;
                volumeSum += v;
            }

            vwma = volumeSum != 0 ? priceVolumeSum / volumeSum : double.NaN;
        }

        AddInternal(new VwmaResult(
            timestamp,
            vwma.NaN2Null()));
    }

    /// <inheritdoc />
    public override void Clear()
    {
        ClearInternal();
        _buffer.Clear();
    }
}

public static partial class Vwma
{
    /// <summary>
    /// Creates a buffer list for Volume Weighted Moving Average (VWMA) calculations.
    /// </summary>
    public static VwmaList ToVwmaList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
        => new(lookbackPeriods) { (IReadOnlyList<IQuote>)quotes };
}
