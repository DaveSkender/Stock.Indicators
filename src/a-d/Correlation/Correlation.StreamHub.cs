namespace Skender.Stock.Indicators;

// CORRELATION (STREAM HUB)

/// <summary>
/// Provides streaming hub calculations for correlation coefficient.
/// </summary>
public class CorrelationHub
    : PairsProvider<IReusable, CorrResult>, ICorrelation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CorrelationHub"/> class.
    /// </summary>
    /// <param name="providerA">The first chain provider.</param>
    /// <param name="providerB">The second chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <exception cref="ArgumentNullException">Thrown when either provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal CorrelationHub(
        IChainProvider<IReusable> providerA,
        IChainProvider<IReusable> providerB,
        int lookbackPeriods) : base(providerA, providerB)
    {
        ArgumentNullException.ThrowIfNull(providerB);
        Correlation.Validate(lookbackPeriods);

        LookbackPeriods = lookbackPeriods;
        Name = $"CORRELATION({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    protected override (CorrResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Check if we have enough data in both caches
        if (HasSufficientData(i, LookbackPeriods))
        {
            // Validate timestamps match
            ValidateTimestampSync(i, item);

            // Extract data arrays for both series
            double[] dataA = new double[LookbackPeriods];
            double[] dataB = new double[LookbackPeriods];

            for (int p = 0; p < LookbackPeriods; p++)
            {
                int index = i - LookbackPeriods + 1 + p;
                dataA[p] = ProviderCache[index].Value;
                dataB[p] = ProviderCacheB[index].Value;
            }

            // Use the existing period correlation calculation
            CorrResult r = Correlation.PeriodCorrelation(item.Timestamp, dataA, dataB);
            return (r, i);
        }
        else
        {
            // Not enough data yet
            CorrResult r = new(Timestamp: item.Timestamp);
            return (r, i);
        }
    }
}

public static partial class Correlation
{
    /// <summary>
    /// Creates a Correlation hub with chain providers sources.
    /// </summary>
    /// <remarks>
    /// Note: This implementation requires both providers to be synchronized (same timestamps).
    /// Both providers must output the same result type.
    /// <para>If providers contain historical data, this hub will fast-forward its cache.</para>
    /// </remarks>
    /// <param name="providerA">The first chain provider.</param>
    /// <param name="providerB">The second chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A chain-sourced instance of <see cref="CorrelationHub"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="lookbackPeriods"/> are invalid.</exception>
    public static CorrelationHub ToCorrelationHub(
        this IChainProvider<IReusable> providerA,
        IChainProvider<IReusable> providerB,
        int lookbackPeriods)
        => new(providerA, providerB, lookbackPeriods);

    // for testing purposes only
    // TODO: should this be public, like the other ToXHub methods?
    internal static CorrelationHub ToCorrelationHub(
        this IReadOnlyList<IQuote> quotesA,
        IReadOnlyList<IQuote> quotesB,
        int lookbackPeriods)
    {
        QuoteHub quoteHubEval = quotesA.ToQuoteHub();
        QuoteHub quoteHubMrkt = quotesB.ToQuoteHub();

        return quoteHubEval.ToCorrelationHub(quoteHubMrkt, lookbackPeriods);
    }
}
