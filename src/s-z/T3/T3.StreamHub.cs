namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the T3 Moving Average indicator.
/// </summary>
public class T3Hub
    : ChainProvider<IReusable, T3Result>, IT3
{
    private readonly string hubName;
    private double lastEma1 = double.NaN;
    private double lastEma2 = double.NaN;
    private double lastEma3 = double.NaN;
    private double lastEma4 = double.NaN;
    private double lastEma5 = double.NaN;
    private double lastEma6 = double.NaN;

    internal T3Hub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods,
        double volumeFactor) : base(provider)
    {
        T3.Validate(lookbackPeriods, volumeFactor);
        LookbackPeriods = lookbackPeriods;
        VolumeFactor = volumeFactor;
        K = 2d / (lookbackPeriods + 1);

        double a = volumeFactor;
        C1 = -a * a * a;
        C2 = (3 * a * a) + (3 * a * a * a);
        C3 = (-6 * a * a) - (3 * a) - (3 * a * a * a);
        C4 = 1 + (3 * a) + (a * a * a) + (3 * a * a);

        hubName = $"T3({lookbackPeriods},{volumeFactor:F1})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public double VolumeFactor { get; init; }

    /// <inheritdoc/>
    public double K { get; private init; }

    /// <inheritdoc/>
    public double C1 { get; private init; }

    /// <inheritdoc/>
    public double C2 { get; private init; }

    /// <inheritdoc/>
    public double C3 { get; private init; }

    /// <inheritdoc/>
    public double C4 { get; private init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (T3Result result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // if out-of-order change (insertion/deletion before current index) occurred
        // invalidate state and backfill from previous cached EMA layers
        if (i > 0 && Cache.Count > i && Cache[i - 1].T3 is not null && (double.IsNaN(lastEma1) || Cache[i - 1].Ema1 != lastEma1))
        {
            lastEma1 = Cache[i - 1].Ema1;
            lastEma2 = Cache[i - 1].Ema2;
            lastEma3 = Cache[i - 1].Ema3;
            lastEma4 = Cache[i - 1].Ema4;
            lastEma5 = Cache[i - 1].Ema5;
            lastEma6 = Cache[i - 1].Ema6;
        }

        double t3 = CalculateIncrement(item.Value, i == 0);

        T3Result r = new(
            Timestamp: item.Timestamp,
            T3: t3.NaN2Null()) {
            Ema1 = lastEma1,
            Ema2 = lastEma2,
            Ema3 = lastEma3,
            Ema4 = lastEma4,
            Ema5 = lastEma5,
            Ema6 = lastEma6
        };

        return (r, i);
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        int i = ProviderCache.IndexGte(timestamp);
        if (i > 0 && Cache.Count > i - 1)
        {
            T3Result prior = Cache[i - 1];
            lastEma1 = prior.Ema1;
            lastEma2 = prior.Ema2;
            lastEma3 = prior.Ema3;
            lastEma4 = prior.Ema4;
            lastEma5 = prior.Ema5;
            lastEma6 = prior.Ema6;
        }
        else
        {
            lastEma1 = lastEma2 = lastEma3 = lastEma4 = lastEma5 = lastEma6 = double.NaN;
        }
    }

    private double CalculateIncrement(double value, bool isFirst)
    {
        // re/seed values on first data point
        if (isFirst || double.IsNaN(lastEma6))
        {
            lastEma1 = lastEma2 = lastEma3 = lastEma4 = lastEma5 = lastEma6 = value;
        }
        else
        {
            lastEma1 = Ema.Increment(K, lastEma1, value);
            lastEma2 = Ema.Increment(K, lastEma2, lastEma1);
            lastEma3 = Ema.Increment(K, lastEma3, lastEma2);
            lastEma4 = Ema.Increment(K, lastEma4, lastEma3);
            lastEma5 = Ema.Increment(K, lastEma5, lastEma4);
            lastEma6 = Ema.Increment(K, lastEma6, lastEma5);
        }

        return (C1 * lastEma6) + (C2 * lastEma5) + (C3 * lastEma4) + (C4 * lastEma3);
    }
}


public static partial class T3
{
    /// <summary>
    /// Creates a T3 streaming hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="volumeFactor">The volume factor for the calculation.</param>
    /// <returns>A T3 hub.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods or volume factor are invalid.</exception>
    public static T3Hub ToT3Hub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods = 5,
        double volumeFactor = 0.7)
        => new(chainProvider, lookbackPeriods, volumeFactor);

    /// <summary>
    /// Creates a T3 hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <param name="volumeFactor">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="T3Hub"/>.</returns>
    public static T3Hub ToT3Hub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 5,
        double volumeFactor = 0.7)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToT3Hub(lookbackPeriods, volumeFactor);
    }

}
