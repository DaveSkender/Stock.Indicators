namespace Skender.Stock.Indicators;

// CORRELATION (STREAM HUB)

/// <summary>
/// Provides methods for calculating the correlation coefficient.
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
    public override string ToString() => Name;

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
    /// Creates a Correlation hub from two synchronized chain providers.
    /// Note: This implementation requires both providers to be synchronized (same timestamps).
    /// Both providers must output the same result type.
    /// For real-time correlation, consider using CorrelationList for more flexibility.
    /// </summary>
    /// <param name="providerA">The first chain provider.</param>
    /// <param name="providerB">The second chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A Correlation hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static CorrelationHub ToCorrelationHub(
        this IChainProvider<IReusable> providerA,
        IChainProvider<IReusable> providerB,
        int lookbackPeriods)
        => new(providerA, providerB, lookbackPeriods);

}
