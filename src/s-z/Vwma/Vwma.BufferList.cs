namespace Skender.Stock.Indicators;

/// <summary>
/// Volume Weighted Moving Average (VWMA) from incremental quote values.
/// </summary>
public class VwmaList : List<VwmaResult>, IVwma, IBufferList
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
        // Update the rolling buffer
        if (_buffer.Count == LookbackPeriods)
        {
            _buffer.Dequeue();
        }

        _buffer.Enqueue((price, volume));

        // Calculate VWMA when we have enough values by recalculating from buffer
        // This matches the precision of the static series implementation
        double? vwma = null;
        if (_buffer.Count == LookbackPeriods)
        {
            double priceVolumeSum = 0;
            double volumeSum = 0;
            foreach (var (p, v) in _buffer)
            {
                priceVolumeSum += p * v;
                volumeSum += v;
            }
            vwma = volumeSum != 0 ? priceVolumeSum / volumeSum : double.NaN;
        }

        base.Add(new VwmaResult(
            timestamp,
            vwma.NaN2Null()));
    }

    /// <inheritdoc />
    public new void Clear()
    {
        base.Clear();
        _buffer.Clear();
    }
}