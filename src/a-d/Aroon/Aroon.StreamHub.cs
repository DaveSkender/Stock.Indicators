namespace Skender.Stock.Indicators;

// AROON OSCILLATOR (STREAM HUB)

/// <summary>
/// Provides methods for calculating the Aroon Oscillator.
/// </summary>
public static partial class Aroon
{
    /// <summary>
    /// Creates an Aroon hub from a quote provider.
    /// </summary>
    /// <typeparam name="T">The type of the quote data.</typeparam>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back. Default is 25.</param>
    /// <returns>An Aroon hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static AroonHub<T> ToAroon<T>(
        this IStreamObservable<T> provider,
        int lookbackPeriods = 25)
        where T : IQuote
        => new(provider, lookbackPeriods);
}

/// <summary>
/// Represents a hub for Aroon Oscillator calculations.
/// </summary>
/// <typeparam name="TIn">The type of the input data.</typeparam>
public class AroonHub<TIn>
    : StreamHub<TIn, AroonResult>, IAroon
    where TIn : IQuote
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="AroonHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal AroonHub(
        IStreamObservable<TIn> provider,
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
        ToIndicator(TIn item, int? indexHint)
    {
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
                TIn d = ProviderCache[p];

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
