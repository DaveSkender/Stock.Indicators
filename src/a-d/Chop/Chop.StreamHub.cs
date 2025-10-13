namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Choppiness Index (CHOP) on a series of quotes.
/// </summary>
public class ChopHub<TIn>
    : ChainProvider<TIn, ChopResult>, IChop
    where TIn : IQuote
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChopHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of periods to use for the lookback window.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal ChopHub(
        IQuoteProvider<TIn> provider,
        int lookbackPeriods) : base(provider)
    {
        Chop.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"CHOP({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (ChopResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // skip first period (no previous close)
        if (i == 0)
        {
            return (new ChopResult(item.Timestamp, null), i);
        }

        double? chop = null;

        // calculate CHOP when we have enough data
        if (i >= LookbackPeriods)
        {
            double sumTrueRange = 0;
            double maxTrueHigh = double.MinValue;
            double minTrueLow = double.MaxValue;

            // iterate over lookback window
            for (int j = 0; j < LookbackPeriods; j++)
            {
                int idx = i - j;
                TIn current = ProviderCache[idx];
                double prevClose = (double)ProviderCache[idx - 1].Close;

                // calculate true high, true low, and true range
                double trueHigh = Math.Max((double)current.High, prevClose);
                double trueLow = Math.Min((double)current.Low, prevClose);
                double trueRange = trueHigh - trueLow;

                sumTrueRange += trueRange;
                maxTrueHigh = Math.Max(maxTrueHigh, trueHigh);
                minTrueLow = Math.Min(minTrueLow, trueLow);
            }

            double range = maxTrueHigh - minTrueLow;

            // calculate CHOP
            if (range != 0)
            {
                chop = 100 * (Math.Log(sumTrueRange / range) / Math.Log(LookbackPeriods));
            }
        }

        // candidate result
        ChopResult r = new(
            Timestamp: item.Timestamp,
            Chop: chop);

        return (r, i);
    }
}


public static partial class Chop
{
    /// <summary>
    /// Creates a Choppiness Index (CHOP) streaming hub from a quote provider.
    /// </summary>
    /// <typeparam name="TIn">The type of the input quote.</typeparam>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of periods to use for the lookback window. Default is 14.</param>
    /// <returns>A ChopHub instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static ChopHub<TIn> ToChopHub<TIn>(
        this IQuoteProvider<TIn> quoteProvider,
        int lookbackPeriods = 14)
        where TIn : IQuote
        => new(quoteProvider, lookbackPeriods);

    /// <summary>
    /// Creates a Chop hub from a collection of quotes.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="ChopHub{TQuote}"/>.</returns>
    public static ChopHub<TQuote> ToChopHub<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 14)
        where TQuote : IQuote
    {
        QuoteHub<TQuote> quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToChopHub(lookbackPeriods);
    }

}
