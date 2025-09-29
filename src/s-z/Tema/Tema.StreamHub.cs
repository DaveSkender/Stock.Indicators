namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Triple Exponential Moving Average (TEMA) indicator.
/// </summary>
public static partial class Tema
{
    /// <summary>
    /// Creates a TEMA hub from a chain provider.
    /// </summary>
    /// <typeparam name="T">The type of the reusable data.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A TEMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static TemaHub<T> ToTema<T>(
        this IChainProvider<T> chainProvider,
        int lookbackPeriods)
        where T : IReusable
    {
        ArgumentNullException.ThrowIfNull(chainProvider);
        return new(chainProvider, lookbackPeriods);
    }
}

/// <summary>
/// Represents a hub for Triple Exponential Moving Average (TEMA) calculations.
/// </summary>
/// <typeparam name="TIn">The type of the input data.</typeparam>
public class TemaHub<TIn>
    : ChainProvider<TIn, TemaResult>, ITema
    where TIn : IReusable
{
    private readonly string hubName;
    private double lastEma1 = double.NaN;
    private double lastEma2 = double.NaN;
    private double lastEma3 = double.NaN;

    internal TemaHub(
        IChainProvider<TIn> provider,
        int lookbackPeriods) : base(provider)
    {
        Tema.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);
        hubName = $"TEMA({lookbackPeriods})";
        Reinitialize();
    }

    public int LookbackPeriods { get; init; }
    public double K { get; private init; }
    public override string ToString() => hubName;

    protected override (TemaResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // if out-of-order change (insertion/deletion before current index) occurred
        // invalidate state and backfill from previous cached EMA layers
        if (i > 0 && Cache.Count > i && Cache[i - 1].Tema is not null && (double.IsNaN(lastEma1) || Cache[i - 1].Ema1 != lastEma1))
        {
            lastEma1 = Cache[i - 1].Ema1;
            lastEma2 = Cache[i - 1].Ema2;
            lastEma3 = Cache[i - 1].Ema3;
        }

        double tema = i >= LookbackPeriods - 1
            ? Cache[i - 1].Tema is not null

                // normal
                ? CalculateIncrement(item.Value)

                // re/initialize as SMA
                : InitializeTema(i)

            // warmup periods are never calculable
            : double.NaN;

        TemaResult r = new(
            Timestamp: item.Timestamp,
            Tema: tema.NaN2Null())
        {
            Ema1 = lastEma1,
            Ema2 = lastEma2,
            Ema3 = lastEma3
        };

        return (r, i);
    }

    protected override void RollbackState(DateTime timestamp)
    {
        int i = ProviderCache.IndexGte(timestamp);
        if (i > LookbackPeriods)
        {
            TemaResult prior = Cache[i - 1];
            lastEma1 = prior.Ema1;
            lastEma2 = prior.Ema2;
            lastEma3 = prior.Ema3;
        }
        else
        {
            lastEma1 = lastEma2 = lastEma3 = double.NaN;
        }
    }

    private double InitializeTema(int index)
    {
        double sum = 0;
        for (int p = index - LookbackPeriods + 1; p <= index; p++)
        {
            sum += ProviderCache[p].Value;
        }

        lastEma1 = lastEma2 = lastEma3 = sum / LookbackPeriods;
        return (3 * lastEma1) - (3 * lastEma2) + lastEma3;
    }

    private double CalculateIncrement(double value)
    {
        lastEma1 = Ema.Increment(K, lastEma1, value);
        lastEma2 = Ema.Increment(K, lastEma2, lastEma1);
        lastEma3 = Ema.Increment(K, lastEma3, lastEma2);
        return (3 * lastEma1) - (3 * lastEma2) + lastEma3;
    }
}
