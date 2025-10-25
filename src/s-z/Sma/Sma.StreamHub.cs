namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (STREAM HUB)

/// <summary>
/// Represents a Simple Moving Average (SMA) stream hub.
/// </summary>
public class SmaHub
    : ChainProvider<IReusable, SmaResult>, ISma
{
    #region fields

    private readonly string hubName;

    #endregion

    #region constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SmaHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    internal SmaHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        Sma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"SMA({lookbackPeriods})";

        Reinitialize();
    }

    #endregion

    #region properties

    /// <summary>
    /// Gets the number of lookback periods.
    /// </summary>
    public int LookbackPeriods { get; init; }

    #endregion

    #region methods

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (SmaResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Calculate SMA efficiently using a rolling window over ProviderCache
        // This is O(lookbackPeriods) which is constant for a given configuration
        // and maintains exact precision with Series implementation
        double? sma = null;
        if (i >= LookbackPeriods - 1)
        {
            double sum = 0;
            bool hasNaN = false;

            for (int p = i - LookbackPeriods + 1; p <= i; p++)
            {
                double value = ProviderCache[p].Value;
                if (double.IsNaN(value))
                {
                    hasNaN = true;
                    break;
                }

                sum += value;
            }

            sma = hasNaN ? null : sum / LookbackPeriods;
        }

        // candidate result
        SmaResult r = new(
            Timestamp: item.Timestamp,
            Sma: sma);

        return (r, i);
    }

    #endregion
}

/// <summary>
/// Provides methods for creating SMA hubs.
/// </summary>
public static partial class Sma
{
    /// <summary>
    /// Converts the chain provider to an SMA hub.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <returns>An SMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static SmaHub ToSmaHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods)
             => new(chainProvider, lookbackPeriods);

    /// <summary>
    /// Creates an SMA hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <returns>An instance of <see cref="SmaHub"/>.</returns>
    public static SmaHub ToSmaHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToSmaHub(lookbackPeriods);
    }
}
