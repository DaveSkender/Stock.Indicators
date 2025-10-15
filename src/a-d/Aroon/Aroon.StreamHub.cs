namespace Skender.Stock.Indicators;

// AROON OSCILLATOR (STREAM HUB)

/// <summary>
/// Represents a hub for Aroon Oscillator calculations.
/// </summary>
public class AroonHub
    : ChainProvider<IQuote, AroonResult>, IAroon
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="AroonHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal AroonHub(
        IQuoteProvider<IQuote> provider,
        int lookbackPeriods) : base(provider)
    {
        Aroon.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"AROON({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (AroonResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double? aroonUp = null;
        double? aroonDown = null;

        // Calculate Aroon when we have enough data
        if (i >= LookbackPeriods)
        {
            decimal lastHighPrice = 0;
            decimal lastLowPrice = decimal.MaxValue;
            int lastHighIndex = 0;
            int lastLowIndex = 0;

            // Look back over the specified period
            for (int p = i - LookbackPeriods; p <= i; p++)
            {
                IQuote d = ProviderCache[p];

                if (d.High > lastHighPrice)
                {
                    lastHighPrice = d.High;
                    lastHighIndex = p;
                }

                if (d.Low < lastLowPrice)
                {
                    lastLowPrice = d.Low;
                    lastLowIndex = p;
                }
            }

            aroonUp = 100d * (LookbackPeriods - (i - lastHighIndex)) / LookbackPeriods;
            aroonDown = 100d * (LookbackPeriods - (i - lastLowIndex)) / LookbackPeriods;
        }

        // Candidate result
        AroonResult r = new(
            Timestamp: item.Timestamp,
            AroonUp: aroonUp,
            AroonDown: aroonDown,
            Oscillator: aroonUp - aroonDown);

        return (r, i);
    }
}

/// <summary>
/// Provides methods for calculating the Aroon Oscillator.
/// </summary>
public static partial class Aroon
{
    /// <summary>
    /// Creates an Aroon hub from a quote provider.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back. Default is 25.</param>
    /// <returns>An Aroon hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static AroonHub ToAroonHub(
        this IQuoteProvider<IQuote> provider,
        int lookbackPeriods = 25)
        => new(provider, lookbackPeriods);

    /// <summary>
    /// Creates an Aroon hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods to look back. Default is 25.</param>
    /// <returns>An instance of <see cref="AroonHub"/>.</returns>
    public static AroonHub ToAroonHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 25)
        => quotes.ToQuoteHub().ToAroonHub(lookbackPeriods);
}
