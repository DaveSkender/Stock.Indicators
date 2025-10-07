namespace Skender.Stock.Indicators;

/// <summary>
/// Donchian Channels from incremental quotes.
/// </summary>
public class DonchianList : BufferList<DonchianResult>, IBufferList
{
    private readonly Queue<decimal> highBuffer;
    private readonly Queue<decimal> lowBuffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="DonchianList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public DonchianList(int lookbackPeriods = 20)
    {
        Donchian.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        highBuffer = new Queue<decimal>(lookbackPeriods);
        lowBuffer = new Queue<decimal>(lookbackPeriods);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DonchianList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public DonchianList(int lookbackPeriods, IReadOnlyList<IQuote> quotes)
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

        DateTime timestamp = quote.Timestamp;

        // Calculate Donchian when we have enough prior data
        // Note: Donchian looks at PRIOR periods (not including current)
        decimal? upperBand = null;
        decimal? lowerBand = null;
        decimal? centerline = null;
        decimal? width = null;

        if (highBuffer.Count == LookbackPeriods)
        {
            decimal highHigh = decimal.MinValue;
            decimal lowLow = decimal.MaxValue;

            // Find highest high and lowest low in the buffer
            foreach (decimal h in highBuffer)
            {
                if (h > highHigh)
                {
                    highHigh = h;
                }
            }

            foreach (decimal l in lowBuffer)
            {
                if (l < lowLow)
                {
                    lowLow = l;
                }
            }

            upperBand = highHigh;
            lowerBand = lowLow;
            centerline = (upperBand + lowerBand) / 2m;
            width = centerline == 0 ? null : (upperBand - lowerBand) / centerline;
        }

        // Update buffers AFTER calculating (since we look at prior periods)
        highBuffer.Update(LookbackPeriods, quote.High);
        lowBuffer.Update(LookbackPeriods, quote.Low);

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
        ClearInternal();
        highBuffer.Clear();
        lowBuffer.Clear();
    }
}

public static partial class Donchian
{
    /// <summary>
    /// Creates a buffer list for Donchian Channels calculations.
    /// </summary>
    public static DonchianList ToDonchianList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 20)
        where TQuote : IQuote
        => new(lookbackPeriods) { (IReadOnlyList<IQuote>)quotes };
}
