namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Double Exponential Moving Average (DEMA) indicator.
/// </summary>
public static partial class Dema
{
    /// <summary>
    /// Creates a DEMA streaming hub from a chain provider.
    /// </summary>
    /// <typeparam name="T">The type of the reusable data.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A DEMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static DemaHub<T> ToDemaHub<T>(
        this IChainProvider<T> chainProvider,
        int lookbackPeriods = 14)
        where T : IReusable
        => new(chainProvider, lookbackPeriods);
}

/// <summary>
/// Streaming hub for Double Exponential Moving Average (DEMA) calculations.
/// </summary>
/// <typeparam name="TIn">The type of the input data.</typeparam>
public class DemaHub<TIn>
    : ChainProvider<TIn, DemaResult>, IDema
    where TIn : IReusable
{
    private readonly string hubName;
    private double lastEma1 = double.NaN;
    private double lastEma2 = double.NaN;

    /// <summary>
    /// Initializes a new instance of the <see cref="DemaHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal DemaHub(
        IChainProvider<TIn> provider,
        int lookbackPeriods) : base(provider)
    {
        Dema.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);
        hubName = $"DEMA({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public double K { get; private init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (DemaResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        // TODO: Optimize by persisting layered EMA state (ema1, ema2)
        // and implementing a targeted rollback that only recomputes the
        // affected tail segment after edits. See discussion in PR #1433.

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // if out-of-order change (insertion/deletion before current index) occurred
        // invalidate state and backfill from previous cached EMA layers
        if (i > 0 && Cache.Count > i && Cache[i - 1].Dema is not null && (double.IsNaN(lastEma1) || Cache[i - 1].Ema1 != lastEma1))
        {
            lastEma1 = Cache[i - 1].Ema1;
            lastEma2 = Cache[i - 1].Ema2;
        }

        double dema = i >= LookbackPeriods - 1
            ? Cache[i - 1].Dema is not null

                // normal
                ? CalculateIncrement(item.Value)

                // re/initialize as SMA
                : InitializeDema(i)

            // warmup periods are never calculable
            : double.NaN;

        DemaResult r = new(
            Timestamp: item.Timestamp,
            Dema: dema.NaN2Null()) {
            Ema1 = lastEma1,
            Ema2 = lastEma2
        };

        return (r, i);
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        int i = ProviderCache.IndexGte(timestamp);
        if (i > LookbackPeriods)
        {
            DemaResult prior = Cache[i - 1];
            lastEma1 = prior.Ema1;
            lastEma2 = prior.Ema2;
        }
        else
        {
            lastEma1 = lastEma2 = double.NaN;
        }
    }

    private double InitializeDema(int index)
    {
        double sum = 0;
        for (int p = index - LookbackPeriods + 1; p <= index; p++)
        {
            sum += ProviderCache[p].Value;
        }

        lastEma1 = lastEma2 = sum / LookbackPeriods;
        return (2 * lastEma1) - lastEma2;
    }

    private double CalculateIncrement(double value)
    {
        lastEma1 = Ema.Increment(K, lastEma1, value);
        lastEma2 = Ema.Increment(K, lastEma2, lastEma1);
        return (2 * lastEma1) - lastEma2;
    }
}
