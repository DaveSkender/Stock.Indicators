namespace Skender.Stock.Indicators;

// WEIGHTED MOVING AVERAGE (STREAM HUB)

/// <summary>
/// Provides methods for creating WMA hubs.
/// </summary>
public class WmaHub
    : ChainProvider<IReusable, WmaResult>, IWma
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="WmaHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    internal WmaHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        Wma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"WMA({lookbackPeriods})";

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
    protected override (WmaResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int index = indexHint ?? ProviderCache.IndexOf(item, true);

        double wma = Wma.Increment(ProviderCache, LookbackPeriods, index);

        WmaResult result = new(
            Timestamp: item.Timestamp,
            Wma: wma.NaN2Null());

        return (result, index);
    }
}


public static partial class Wma
{
    /// <summary>
    /// Converts the chain provider to a WMA hub.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <returns>A WMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static WmaHub ToWmaHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods)
    {
        ArgumentNullException.ThrowIfNull(chainProvider);
        return new(chainProvider, lookbackPeriods);
    }

    /// <summary>
    /// Creates a Wma hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="WmaHub"/>.</returns>
    public static WmaHub ToWmaHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToWmaHub(lookbackPeriods);
    }

}
