namespace Skender.Stock.Indicators;

// CORRELATION (STREAM HUB)

/// <summary>
/// Provides methods for calculating the correlation coefficient.
/// </summary>
public static partial class Correlation
{
    /// <summary>
    /// Creates a Correlation hub from two synchronized chain providers.
    /// Note: This implementation requires both providers to be synchronized (same timestamps).
    /// Both providers must output the same result type.
    /// For real-time correlation, consider using CorrelationList for more flexibility.
    /// </summary>
    /// <typeparam name="T">The type of the reusable data (must be the same for both providers).</typeparam>
    /// <param name="providerA">The first chain provider.</param>
    /// <param name="providerB">The second chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A Correlation hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static CorrelationHub<T> ToCorrelationHub<T>(
        this IChainProvider<T> providerA,
        IChainProvider<T> providerB,
        int lookbackPeriods)
        where T : IReusable
        => new(providerA, providerB, lookbackPeriods);
}

/// <summary>
/// Represents a hub for Correlation calculations between two synchronized series.
/// </summary>
/// <typeparam name="TIn">The type of the input data.</typeparam>
public class CorrelationHub<TIn>
    : PairsProvider<TIn, CorrResult>, ICorrelation
    where TIn : IReusable
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="CorrelationHub{TIn}"/> class.
    /// </summary>
    /// <param name="providerA">The first chain provider.</param>
    /// <param name="providerB">The second chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when either provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal CorrelationHub(
        IChainProvider<TIn> providerA,
        IChainProvider<TIn> providerB,
        int lookbackPeriods) : base(providerA, providerB)
    {
        ArgumentNullException.ThrowIfNull(providerB);
        Correlation.Validate(lookbackPeriods);

        LookbackPeriods = lookbackPeriods;
        hubName = $"CORRELATION({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (CorrResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
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
