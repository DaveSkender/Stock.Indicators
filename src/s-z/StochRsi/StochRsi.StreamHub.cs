namespace Skender.Stock.Indicators;

// STOCHASTIC RSI (STREAM HUB)


/// <summary>
/// Represents a Stochastic RSI stream hub.
/// </summary>
/// <typeparam name="TIn">The type of the input data.</typeparam>
public sealed class StochRsiHub<TIn>
    : ChainProvider<TIn, StochRsiResult>
    where TIn : IReusable
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="StochRsiHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="rsiPeriods">The number of periods for the RSI calculation.</param>
    /// <param name="stochPeriods">The number of periods for the Stochastic calculation.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <param name="smoothPeriods">The number of periods for smoothing.</param>
    internal StochRsiHub(
        IChainProvider<TIn> provider,
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
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double? stochRsi = null;
        double? signal = null;

        // We need at least rsiPeriods + stochPeriods - 1 items to calculate
        int minRequired = RsiPeriods + StochPeriods - 1;

        if (i >= minRequired)
        {
            // Build a subset of provider cache for StochRSI calculation
            List<TIn> subset = [];
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
    /// <typeparam name="T">The type of the reusable data.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="rsiPeriods">The number of periods for the RSI calculation.</param>
    /// <param name="stochPeriods">The number of periods for the Stochastic calculation.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <param name="smoothPeriods">The number of periods for smoothing (default is 1).</param>
    /// <returns>A Stochastic RSI hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid.</exception>
    public static StochRsiHub<T> ToStochRsiHub<T>(
        this IChainProvider<T> chainProvider,
        int rsiPeriods = 14,
        int stochPeriods = 14,
        int signalPeriods = 3,
        int smoothPeriods = 1)
        where T : IReusable
        => new(chainProvider, rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);


    /// <summary>
    /// Creates a StochRsi hub from a collection of quotes.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="14">Parameter for the calculation.</param>
    /// <param name="14">Parameter for the calculation.</param>
    /// <param name="3">Parameter for the calculation.</param>
    /// <param name="1">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="StochRsiHub{TQuote}"/>.</returns>
    public static StochRsiHub<TQuote> ToStochRsiHub<TQuote>(
        this IReadOnlyList<TQuote> quotes, int rsiPeriods = 14, int stochPeriods = 14, int signalPeriods = 3, int smoothPeriods = 1)
        where TQuote : IQuote
    {
        QuoteHub<TQuote> quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToStochRsiHub(14, 14, 3, 1);
    }
}
