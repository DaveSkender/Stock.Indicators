namespace Skender.Stock.Indicators;

// VOLUME WEIGHTED MOVING AVERAGE (STREAM HUB)

/// <summary>
/// Provides methods for creating VWMA hubs.
/// </summary>
public class VwmaHub
    : ChainProvider<IReusable, VwmaResult>, IVwma
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="VwmaHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    internal VwmaHub(
        IQuoteProvider<IQuote> provider,
        int lookbackPeriods) : base(provider)
    {
        Vwma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"VWMA({lookbackPeriods})";

        Reinitialize();
    }

    /// <summary>
    /// Gets the number of lookback periods.
    /// </summary>
    public int LookbackPeriods { get; init; }

    // METHODS

    /// <inheritdoc />
    public override string ToString() => hubName;

    /// <inheritdoc />
    protected override (VwmaResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int index = indexHint ?? ProviderCache.IndexOf(item, true);

        // Calculate VWMA efficiently using a rolling window over ProviderCache
        // This is O(lookbackPeriods) which is constant for a given configuration
        // and maintains exact precision with Series implementation
        double? vwma = null;

        if (index >= LookbackPeriods - 1)
        {
            double priceVolumeSum = 0;
            double volumeSum = 0;

            for (int p = index - LookbackPeriods + 1; p <= index; p++)
            {
                IQuote quote = (IQuote)ProviderCache[p];
                double price = (double)quote.Close;
                double volume = (double)quote.Volume;

                priceVolumeSum += price * volume;
                volumeSum += volume;
            }

            vwma = volumeSum != 0 ? priceVolumeSum / volumeSum : null;
        }

        VwmaResult result = new(
            Timestamp: item.Timestamp,
            Vwma: vwma);

        return (result, index);
    }
}


public static partial class Vwma
{
    /// <summary>
    /// Converts the quote provider to a VWMA hub.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <returns>A VWMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static VwmaHub ToVwmaHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int lookbackPeriods)
    {
        ArgumentNullException.ThrowIfNull(quoteProvider);
        return new(quoteProvider, lookbackPeriods);
    }

    /// <summary>
    /// Creates a Vwma hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="VwmaHub"/>.</returns>
    public static VwmaHub ToVwmaHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToVwmaHub(lookbackPeriods);
    }

}
