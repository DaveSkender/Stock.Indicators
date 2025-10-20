namespace Skender.Stock.Indicators;

// FORCE INDEX (STREAM HUB)

/// <summary>
/// Provides methods for creating Force Index hubs.
/// </summary>
public class ForceIndexHub
    : ChainProvider<IReusable, ForceIndexResult>, IForceIndex
{
    private readonly string hubName;
    private readonly double _k;

    /// <summary>
    /// Initializes a new instance of the <see cref="ForceIndexHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    internal ForceIndexHub(
        IQuoteProvider<IQuote> provider,
        int lookbackPeriods) : base(provider)
    {
        ForceIndex.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        _k = 2d / (lookbackPeriods + 1);
        hubName = $"FORCE({lookbackPeriods})";

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
    protected override (ForceIndexResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int index = indexHint ?? ProviderCache.IndexOf(item, true);

        double? fi = null;

        // skip first period
        if (index > 0)
        {
            // get current and previous quotes
            IQuote currentQuote = (IQuote)ProviderCache[index];
            IQuote previousQuote = (IQuote)ProviderCache[index - 1];

            // calculate raw Force Index
            double rawFi = (double)currentQuote.Volume * ((double)currentQuote.Close - (double)previousQuote.Close);

            // calculate EMA
            if (index > LookbackPeriods)
            {
                // Use previous cached result for incremental EMA
                double? prevFi = Cache[index - 1].ForceIndex;
                if (prevFi.HasValue)
                {
                    fi = prevFi + (_k * (rawFi - prevFi));
                }
            }
            // initialization period
            else
            {
                // Calculate sum of raw FI for initialization
                double sumRawFi = 0;
                for (int j = 1; j <= index; j++)
                {
                    IQuote curr = (IQuote)ProviderCache[j];
                    IQuote prev = (IQuote)ProviderCache[j - 1];
                    sumRawFi += (double)curr.Volume * ((double)curr.Close - (double)prev.Close);
                }

                // first EMA value
                if (index == LookbackPeriods)
                {
                    fi = sumRawFi / LookbackPeriods;
                }
            }
        }

        ForceIndexResult result = new(
            Timestamp: item.Timestamp,
            ForceIndex: fi);

        return (result, index);
    }
}


public static partial class ForceIndex
{
    /// <summary>
    /// Converts the quote provider to a Force Index hub.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <returns>A Force Index hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static ForceIndexHub ToForceIndexHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int lookbackPeriods = 2)
    {
        ArgumentNullException.ThrowIfNull(quoteProvider);
        return new(quoteProvider, lookbackPeriods);
    }

    /// <summary>
    /// Creates a Force Index hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="ForceIndexHub"/>.</returns>
    public static ForceIndexHub ToForceIndexHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 2)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToForceIndexHub(lookbackPeriods);
    }

}
