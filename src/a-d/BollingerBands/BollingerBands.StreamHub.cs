namespace Skender.Stock.Indicators;

// BOLLINGER BANDS (STREAM HUB)

/// <summary>
/// Provides methods for creating Bollinger Bands hubs.
/// </summary>
public class BollingerBandsHub
    : ChainProvider<IReusable, BollingerBandsResult>, IBollingerBands
{


    /// <summary>
    /// Initializes a new instance of the <see cref="BollingerBandsHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="standardDeviations">The number of standard deviations.</param>
    internal BollingerBandsHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods,
        double standardDeviations) : base(provider)
    {
        BollingerBands.Validate(lookbackPeriods, standardDeviations);
        LookbackPeriods = lookbackPeriods;
        StandardDeviations = standardDeviations;
        Name = $"BB({lookbackPeriods},{standardDeviations})";

        Reinitialize();
    }

    /// <summary>
    /// Gets the number of lookback periods.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <summary>
    /// Gets the number of standard deviations.
    /// </summary>
    public double StandardDeviations { get; }

    /// <inheritdoc/>
    public override string ToString() => Name;

    /// <inheritdoc/>
    protected override (BollingerBandsResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // candidate result using Increment method like SMA
        BollingerBandsResult r = BollingerBands.Increment(
            ProviderCache,
            LookbackPeriods,
            StandardDeviations,
            i);

        return (r, i);
    }

}

public static partial class BollingerBands
{
    /// <summary>
    /// Converts the chain provider to a Bollinger Bands hub.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="standardDeviations">The number of standard deviations.</param>
    /// <returns>A Bollinger Bands hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the parameters are invalid.</exception>
    public static BollingerBandsHub ToBollingerBandsHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods = 20,
        double standardDeviations = 2)
             => new(chainProvider, lookbackPeriods, standardDeviations);

    /// <summary>
    /// Creates a BollingerBands hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="standardDeviations">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="BollingerBandsHub"/>.</returns>
    public static BollingerBandsHub ToBollingerBandsHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 20,
        double standardDeviations = 2)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToBollingerBandsHub(lookbackPeriods, standardDeviations);
    }

}
