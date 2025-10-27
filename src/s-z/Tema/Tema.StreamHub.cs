namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Triple Exponential Moving Average (TEMA) indicator.
/// </summary>
public class TemaHub
    : ChainProvider<IReusable, TemaResult>, ITema
{
    private readonly string hubName;
    private double lastEma1 = double.NaN;
    private double lastEma2 = double.NaN;
    private double lastEma3 = double.NaN;

    internal TemaHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        Tema.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);
        hubName = $"TEMA({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public double K { get; private init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (TemaResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        // TODO: Optimize by persisting layered EMA state (ema1, ema2, ema3)
        // and implementing a targeted rollback that only recomputes the
        // affected tail segment after edits. See discussion in PR #1433.

        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // if out-of-order change (insertion/deletion before current index) occurred
        // invalidate state and backfill from previous cached EMA layers
        if (i > 0 && Cache.Count > i && Cache[i - 1].Tema is not null && (double.IsNaN(lastEma1) || Cache[i - 1].Ema1 != lastEma1))
        {
            lastEma1 = Cache[i - 1].Ema1;
            lastEma2 = Cache[i - 1].Ema2;
            lastEma3 = Cache[i - 1].Ema3;
        }

        double tema;
        if (i >= LookbackPeriods - 1)
        {
            if (Cache[i - 1].Tema is not null)
            {
                tema = CalculateIncrement(item.Value);
            }
            else
            {
                tema = InitializeTema(i);
            }
        }
        else
        {
            tema = double.NaN;
        }

        TemaResult r = new(
            Timestamp: item.Timestamp,
            Tema: tema.NaN2Null()) {
            Ema1 = lastEma1,
            Ema2 = lastEma2,
            Ema3 = lastEma3
        };

        return (r, i);
    }

    /// <inheritdoc/>
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


public static partial class Tema
{
    /// <summary>
    /// Creates a TEMA streaming hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A TEMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static TemaHub ToTemaHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods = 20)
        => new(chainProvider, lookbackPeriods);

    /// <summary>
    /// Creates a Tema hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="TemaHub"/>.</returns>
    public static TemaHub ToTemaHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 20)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToTemaHub(lookbackPeriods);
    }

}
