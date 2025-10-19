namespace Skender.Stock.Indicators;


/// <summary>
/// Streaming hub for Triple Exponential Moving Average Oscillator (TRIX) calculations.
/// </summary>
public class TrixHub
    : ChainProvider<IReusable, TrixResult>, ITrix
{
    private readonly string hubName;
    private double lastEma1 = double.NaN;
    private double lastEma2 = double.NaN;
    private double lastEma3 = double.NaN;

    internal TrixHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        Trix.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);
        hubName = $"TRIX({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public double K { get; private init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (TrixResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // if out-of-order change (insertion/deletion before current index) occurred
        // restore state from previous cached result
        if (i > 0 && Cache.Count > i && Cache[i - 1].Trix is not null && (double.IsNaN(lastEma1) || Cache[i - 1].Ema1 != lastEma1))
        {
            TrixResult prior = Cache[i - 1];
            lastEma1 = prior.Ema1;
            lastEma2 = prior.Ema2;
            lastEma3 = prior.Ema3!.Value;
        }

        double ema1;
        double ema2;
        double ema3;
        double? ema3Result = null;
        double? trix = null;

        if (i >= LookbackPeriods - 1)
        {
            // Check if we should initialize or calculate normally
            if (double.IsNaN(lastEma3))
            {
                // re/initialize as SMA
                (ema1, ema2, ema3) = InitializeTrix(i);
                // First period after initialization - no Ema3 or TRIX output yet
            }
            else
            {
                // normal calculation
                (ema1, ema2, ema3, ema3Result, trix) = CalculateIncrement(item.Value);
            }
        }
        else
        {
            // warmup periods are never calculable
            ema1 = ema2 = double.NaN;
        }

        TrixResult r = new(
            Timestamp: item.Timestamp,
            Ema3: ema3Result,
            Trix: trix) {
            Ema1 = ema1,
            Ema2 = ema2
        };

        return (r, i);
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        int i = ProviderCache.IndexGte(timestamp);
        if (i > LookbackPeriods)
        {
            TrixResult prior = Cache[i - 1];
            lastEma1 = prior.Ema1;
            lastEma2 = prior.Ema2;
            lastEma3 = prior.Ema3 ?? double.NaN;
        }
        else
        {
            lastEma1 = lastEma2 = lastEma3 = double.NaN;
        }
    }

    private (double, double, double) InitializeTrix(int index)
    {
        double sum = 0;
        for (int p = index - LookbackPeriods + 1; p <= index; p++)
        {
            sum += ProviderCache[p].Value;
        }

        lastEma1 = lastEma2 = lastEma3 = sum / LookbackPeriods;
        return (lastEma1, lastEma2, lastEma3);
    }

    private (double, double, double, double?, double?) CalculateIncrement(double value)
    {
        lastEma1 = Ema.Increment(K, lastEma1, value);
        lastEma2 = Ema.Increment(K, lastEma2, lastEma1);

        // Store previous Ema3 before updating
        double prevEma3 = lastEma3;
        lastEma3 = Ema.Increment(K, lastEma3, lastEma2);

        // Calculate TRIX
        double trix = 100 * (lastEma3 - prevEma3) / prevEma3;

        return (lastEma1, lastEma2, lastEma3, lastEma3, trix);
    }

}


public static partial class Trix
{
    /// <summary>
    /// Creates a TRIX streaming hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A TRIX hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static TrixHub ToTrix(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods = 14)
        => new(chainProvider, lookbackPeriods);

    /// <summary>
    /// Creates a Trix hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods"></param>
    /// <returns>An instance of <see cref="TrixHub"/>.</returns>
    public static TrixHub ToTrixHub(
        this IReadOnlyList<IQuote> quotes, int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToTrix(14);
    }
}

