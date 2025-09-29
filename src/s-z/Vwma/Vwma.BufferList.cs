namespace Skender.Stock.Indicators;

/// <summary>
/// Volume Weighted Moving Average (VWMA) from incremental quote values.
/// </summary>
public class VwmaList : List<VwmaResult>, IVwma, IBufferList
{
    private readonly Queue<(double price, double volume)> _buffer;
    private double _priceVolumeSum;
    private double _volumeSum;

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
        _priceVolumeSum = 0;
        _volumeSum = 0;
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
        // update buffer - remove oldest values if at capacity
        if (_buffer.Count == LookbackPeriods)
        {
            var (oldPrice, oldVolume) = _buffer.Dequeue();
            _priceVolumeSum -= oldPrice * oldVolume;
            _volumeSum -= oldVolume;
        }

        // add new values
        _buffer.Enqueue((price, volume));
        _priceVolumeSum += price * volume;
        _volumeSum += volume;

        // calculate VWMA (or null if insufficient data or zero volume)
        double? vwma = _buffer.Count >= LookbackPeriods && _volumeSum != 0
            ? _priceVolumeSum / _volumeSum
            : null;

        base.Add(new VwmaResult(
            timestamp,
            vwma));
    }

    /// <inheritdoc />
    public new void Clear()
    {
        base.Clear();
        _buffer.Clear();
        _priceVolumeSum = 0;
        _volumeSum = 0;
    }
}