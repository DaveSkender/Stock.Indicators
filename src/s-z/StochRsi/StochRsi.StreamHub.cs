namespace Skender.Stock.Indicators;

// STOCHASTIC RSI (STREAM HUB)


/// <summary>
/// Represents a Stochastic RSI stream hub.
/// </summary>
public sealed class StochRsiHub
    : ChainProvider<IReusable, StochRsiResult>
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="StochRsiHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="rsiPeriods">The number of periods for the RSI calculation.</param>
    /// <param name="stochPeriods">The number of periods for the Stochastic calculation.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <param name="smoothPeriods">The number of periods for smoothing.</param>
    internal StochRsiHub(
        IChainProvider<IReusable> provider,
        int rsiPeriods = 14,
        int stochPeriods = 14,
        int signalPeriods = 3,
        int smoothPeriods = 1) : base(provider)
    {
        StochRsi.Validate(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        RsiPeriods = rsiPeriods;
        StochPeriods = stochPeriods;
        SignalPeriods = signalPeriods;
        SmoothPeriods = smoothPeriods;

        hubName = $"STOCH-RSI({rsiPeriods},{stochPeriods},{signalPeriods},{smoothPeriods})";

        Reinitialize();
    }

    /// <summary>
    /// Gets the number of periods for the RSI calculation.
    /// </summary>
    public int RsiPeriods { get; init; }

    /// <summary>
    /// Gets the number of periods for the Stochastic calculation.
    /// </summary>
    public int StochPeriods { get; init; }

    /// <summary>
    /// Gets the number of periods for the signal line.
    /// </summary>
    public int SignalPeriods { get; init; }

    /// <summary>
    /// Gets the number of periods for smoothing.
    /// </summary>
    public int SmoothPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (StochRsiResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double? stochRsi = null;
        double? signal = null;

        // We need at least rsiPeriods + stochPeriods - 1 items to calculate
        int minRequired = RsiPeriods + StochPeriods - 1;

        if (i >= minRequired)
        {
            // Build a subset of provider cache for StochRSI calculation
            List<IReusable> subset = [];
            for (int k = 0; k <= i; k++)
            {
                subset.Add(ProviderCache[k]);
            }

            // Use the existing series calculation
            IReadOnlyList<StochRsiResult> seriesResults = subset.ToStochRsi(RsiPeriods, StochPeriods, SignalPeriods, SmoothPeriods);
            StochRsiResult? latestResult = seriesResults.Count > 0 ? seriesResults[seriesResults.Count - 1] : null;

            stochRsi = latestResult?.StochRsi;
            signal = latestResult?.Signal;
        }

        // candidate result
        StochRsiResult result = new(
            Timestamp: item.Timestamp,
            StochRsi: stochRsi,
            Signal: signal);

        return (result, i);
    }
}

public static partial class StochRsi
{
    /// <summary>
    /// Converts the chain provider to a Stochastic RSI hub.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="rsiPeriods">The number of periods for the RSI calculation.</param>
    /// <param name="stochPeriods">The number of periods for the Stochastic calculation.</param>
    /// <param name="kPeriods">The number of periods for %K smoothing.</param>
    /// <param name="dPeriods">The number of periods for %D smoothing.</param>
    /// <returns>A Stochastic RSI hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid.</exception>
    public static StochRsiHub ToStochRsiHub(
        this IChainProvider<IReusable> chainProvider,
        int rsiPeriods = 14,
        int stochPeriods = 14,
        int kPeriods = 3,
        int dPeriods = 3)
        => new(chainProvider, rsiPeriods, stochPeriods, kPeriods, dPeriods);


    /// <summary>
    /// Creates a StochRsi hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="rsiPeriods"></param>
    /// <param name="stochPeriods"></param>
    /// <param name="signalPeriods"></param>
    /// <param name="smoothPeriods"></param>
    /// <returns>An instance of <see cref="StochRsiHub"/>.</returns>
    public static StochRsiHub ToStochRsiHub(
        this IReadOnlyList<IQuote> quotes, int rsiPeriods = 14, int stochPeriods = 14, int signalPeriods = 3, int smoothPeriods = 1)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToStochRsiHub(14, 14, 3, 1);
    }
}
