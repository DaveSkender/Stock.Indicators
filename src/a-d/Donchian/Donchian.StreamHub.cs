namespace Skender.Stock.Indicators;

// DONCHIAN CHANNELS (STREAM HUB)

/// <summary>
/// Provides methods for calculating the Donchian Channels using a stream hub.
/// </summary>
public static partial class Donchian
{
    /// <summary>
    /// Creates a Donchian Channels streaming hub from a quotes provider.
    /// </summary>
    /// <typeparam name="TIn">The type of the input quote.</typeparam>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation. Default is 20.</param>
    /// <returns>An instance of <see cref="DonchianHub{TIn}"/>.</returns>
    public static DonchianHub<TIn> ToDonchianHub<TIn>(
        this IQuoteProvider<TIn> quoteProvider,
        int lookbackPeriods = 20)
        where TIn : IQuote
        => new(quoteProvider, lookbackPeriods);
}

/// <summary>
/// Represents a stream hub for calculating the Donchian Channels.
/// </summary>
/// <typeparam name="TIn">The type of the input quote.</typeparam>
public class DonchianHub<TIn>
    : StreamHub<TIn, DonchianResult>, IDonchian
    where TIn : IQuote
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="DonchianHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    internal DonchianHub(
        IQuoteProvider<TIn> provider,
        int lookbackPeriods) : base(provider)
    {
        Donchian.Validate(lookbackPeriods);

        LookbackPeriods = lookbackPeriods;
        hubName = $"DONCHIAN({lookbackPeriods})";

        Reinitialize();
    }

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (DonchianResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // handle warmup periods
        // Note: Donchian looks at PRIOR periods (not including current)
        if (i < LookbackPeriods)
        {
            return (new DonchianResult(item.Timestamp), i);
        }

        // find highest high and lowest low in prior periods
        decimal highHigh = decimal.MinValue;
        decimal lowLow = decimal.MaxValue;

        for (int p = i - LookbackPeriods; p < i; p++)
        {
            TIn quote = ProviderCache[p];

            if (quote.High > highHigh)
            {
                highHigh = quote.High;
            }

            if (quote.Low < lowLow)
            {
                lowLow = quote.Low;
            }
        }

        decimal upperBand = highHigh;
        decimal lowerBand = lowLow;
        decimal centerline = (upperBand + lowerBand) / 2m;
        decimal? width = centerline == 0 ? null : (upperBand - lowerBand) / centerline;

        DonchianResult r = new(
            Timestamp: item.Timestamp,
            UpperBand: upperBand,
            Centerline: centerline,
            LowerBand: lowerBand,
            Width: width);

        return (r, i);
    }
}
