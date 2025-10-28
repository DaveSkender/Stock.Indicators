namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Kaufman's Adaptive Moving Average (KAMA) indicator.
/// </summary>
public class KamaHub
    : ChainProvider<IReusable, KamaResult>, IKama
{
    private readonly string hubName;
    private readonly double _scFast;
    private readonly double _scSlow;

    /// <summary>
    /// Initializes a new instance of the <see cref="KamaHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="erPeriods">The number of periods for the Efficiency Ratio (ER).</param>
    /// <param name="fastPeriods">The number of periods for the fast EMA.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are out of their valid range.</exception>
    internal KamaHub(
        IChainProvider<IReusable> provider,
        int erPeriods,
        int fastPeriods,
        int slowPeriods) : base(provider)
    {
        Kama.Validate(erPeriods, fastPeriods, slowPeriods);
        ErPeriods = erPeriods;
        FastPeriods = fastPeriods;
        SlowPeriods = slowPeriods;

        _scFast = 2d / (fastPeriods + 1);
        _scSlow = 2d / (slowPeriods + 1);

        hubName = $"KAMA({erPeriods},{fastPeriods},{slowPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int ErPeriods { get; init; }

    /// <inheritdoc/>
    public int FastPeriods { get; init; }

    /// <inheritdoc/>
    public int SlowPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (KamaResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // skip incalculable periods
        if (i < ErPeriods - 1)
        {
            KamaResult r = new(item.Timestamp);
            return (r, i);
        }

        double er;
        double kama;

        if (i > ErPeriods - 1 && Cache[i - 1].Kama is not null)
        {
            double newVal = item.Value;

            // ER period change
            double change = Math.Abs(newVal - ProviderCache[i - ErPeriods].Value);

            // volatility
            double sumPv = 0;
            for (int p = i - ErPeriods + 1; p <= i; p++)
            {
                sumPv += Math.Abs(ProviderCache[p].Value - ProviderCache[p - 1].Value);
            }

            if (sumPv != 0)
            {
                // efficiency ratio
                er = change / sumPv;

                // smoothing constant
                double sc = (er * (_scFast - _scSlow)) + _scSlow;  // squared later

                // kama calculation
                kama = Cache[i - 1].Value + (sc * sc * (newVal - Cache[i - 1].Value));
            }

            // handle flatline case
            else
            {
                er = 0;
                kama = item.Value;
            }
        }

        // re/initialize
        else
        {
            er = double.NaN;
            kama = item.Value;
        }

        // candidate result
        KamaResult result = new(
            Timestamp: item.Timestamp,
            Er: er.NaN2Null(),
            Kama: kama.NaN2Null());

        return (result, i);
    }
}


public static partial class Kama
{
    /// <summary>
    /// Creates a KAMA streaming hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="erPeriods">The number of periods for the Efficiency Ratio (ER).</param>
    /// <param name="fastPeriods">The number of periods for the fast EMA.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA.</param>
    /// <returns>A KAMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are out of their valid range.</exception>
    public static KamaHub ToKamaHub(
        this IChainProvider<IReusable> chainProvider,
        int erPeriods = 10,
        int fastPeriods = 2,
        int slowPeriods = 30)
        => new(chainProvider, erPeriods, fastPeriods, slowPeriods);

    /// <summary>
    /// Creates a Kama hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="erPeriods">Parameter for the calculation.</param>
    /// <param name="fastPeriods">Parameter for the calculation.</param>
    /// <param name="slowPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="KamaHub"/>.</returns>
    public static KamaHub ToKamaHub(
        this IReadOnlyList<IQuote> quotes,
        int erPeriods = 10,
        int fastPeriods = 2,
        int slowPeriods = 30)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToKamaHub(erPeriods, fastPeriods, slowPeriods);
    }

}
