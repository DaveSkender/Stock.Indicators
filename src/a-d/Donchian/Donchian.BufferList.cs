namespace Skender.Stock.Indicators;

/// <summary>
/// Donchian Channels from incremental quotes.
/// </summary>
public class DonchianList : BufferList<DonchianResult>, IIncrementFromQuote
{
    private readonly Queue<(decimal High, decimal Low)> _buffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="DonchianList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="lookbackPeriods"/> is invalid.</exception>
    public DonchianList(int lookbackPeriods = 20)
    {
        Donchian.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        _buffer = new Queue<(decimal, decimal)>(lookbackPeriods);

        Name = $"DONCHIAN({lookbackPeriods})";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DonchianList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="lookbackPeriods"/> is invalid.</exception>
    public DonchianList(int lookbackPeriods, IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods) => Add(quotes);

    /// <inheritdoc />
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        DateTime timestamp = quote.Timestamp;

        // Calculate Donchian when we have enough prior data
        // Note: Donchian looks at PRIOR periods (not including current)
        decimal? upperBand = null;
        decimal? lowerBand = null;
        decimal? centerline = null;
        decimal? width = null;

        if (_buffer.Count == LookbackPeriods)
        {
            decimal highHigh = decimal.MinValue;
            decimal lowLow = decimal.MaxValue;

            // Find highest high and lowest low in the buffer
            foreach ((decimal High, decimal Low) in _buffer)
            {
                if (High > highHigh)
                {
                    highHigh = High;
                }

                if (Low < lowLow)
                {
                    lowLow = Low;
                }
            }

            upperBand = highHigh;
            lowerBand = lowLow;
            centerline = (upperBand + lowerBand) / 2m;
            width = centerline == 0 ? null : (upperBand - lowerBand) / centerline;
        }

        // Update buffer AFTER calculating (since we look at prior periods)
        _buffer.Update(LookbackPeriods, (quote.High, quote.Low));

        AddInternal(new DonchianResult(
            Timestamp: timestamp,
            UpperBand: upperBand,
            Centerline: centerline,
            LowerBand: lowerBand,
            Width: width));
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
    /// Clears the list and resets internal buffers so the instance can be reused.
    /// </summary>
    public override void Clear()
    {
        base.Clear();
        _buffer.Clear();
    }
}

public static partial class Donchian
{
    /// <summary>
    /// Creates a buffer list for Donchian Channels calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    public static DonchianList ToDonchianList(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 20)
        => new(lookbackPeriods) { quotes };
}
