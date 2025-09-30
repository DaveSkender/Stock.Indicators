namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Relative Strength Index (RSI) indicator.
/// </summary>
public static partial class Rsi
{
    /// <summary>
    /// Creates an RSI hub from a chain provider.
    /// </summary>
    /// <typeparam name="T">The type of the reusable data.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>An RSI hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static RsiHub<T> ToRsi<T>(
        this IChainProvider<T> chainProvider,
        int lookbackPeriods = 14)
        where T : IReusable
        => new(chainProvider, lookbackPeriods);
}

/// <summary>
/// Represents a hub for Relative Strength Index (RSI) calculations.
/// </summary>
/// <typeparam name="TIn">The type of the input data.</typeparam>
public class RsiHub<TIn>
    : ChainProvider<TIn, RsiResult>, IRsi
    where TIn : IReusable
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="RsiHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal RsiHub(
        IChainProvider<TIn> provider,
        int lookbackPeriods) : base(provider)
    {
        Rsi.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"RSI({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (RsiResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double? rsi = null;

        if (i >= LookbackPeriods)
        {
            // Build a subset of provider cache for RSI calculation
            List<TIn> subset = new();
            for (int k = 0; k <= i; k++)
            {
                subset.Add(ProviderCache[k]);
            }

            // Use the existing series calculation
            var seriesResults = subset.ToRsi(LookbackPeriods);
            var latestResult = seriesResults.LastOrDefault();

            rsi = latestResult?.Rsi;
        }

        // candidate result
        RsiResult r = new(
            Timestamp: item.Timestamp,
            Rsi: rsi);

        return (r, i);
    }
}
