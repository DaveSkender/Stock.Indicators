namespace Skender.Stock.Indicators;

// FORCE INDEX (STREAM HUB)

/// <summary>
/// Provides streaming hub for Force Index calculations.
/// </summary>
public class ForceIndexHub
    : ChainProvider<IReusable, ForceIndexResult>, IForceIndex
{
    private readonly double _k;

    internal ForceIndexHub(
        IQuoteProvider<IQuote> provider,
        int lookbackPeriods) : base(provider)
    {
        ForceIndex.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        _k = 2d / (lookbackPeriods + 1);
        Name = $"FORCE({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public override string ToString() => Name;

    /// <inheritdoc />
    protected override (ForceIndexResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double fi = double.NaN;

        // skip first period (need prior quote for delta)
        if (i > 0)
        {
            // get current and previous quotes
            IQuote currentQuote = (IQuote)ProviderCache[i];
            IQuote previousQuote = (IQuote)ProviderCache[i - 1];

            // calculate raw Force Index
            double rawFi = (double)currentQuote.Volume
                * ((double)currentQuote.Close - (double)previousQuote.Close);

            if (i >= LookbackPeriods)
            {
                // Check if previous result has a valid ForceIndex for incremental update
                fi = Cache[i - 1].ForceIndex is not null

                    // Incremental O(1) EMA update
                    ? Ema.Increment(_k, Cache[i - 1].Value, rawFi)

                    // First EMA value - calculate as SMA of raw Force Index values
                    : CalcInitialSma(i);
            }
        }

        ForceIndexResult result = new(
            Timestamp: item.Timestamp,
            ForceIndex: fi.NaN2Null());

        return (result, i);
    }

    /// <summary>
    /// Calculates the initial SMA of raw Force Index values for EMA seeding.
    /// </summary>
    /// <param name="endIndex">The ending index for SMA calculation.</param>
    /// <returns>The SMA value.</returns>
    private double CalcInitialSma(int endIndex)
    {
        double sum = 0;
        int startIndex = endIndex - LookbackPeriods + 1;

        for (int j = startIndex; j <= endIndex; j++)
        {
            IQuote curr = (IQuote)ProviderCache[j];
            IQuote prev = (IQuote)ProviderCache[j - 1];
            sum += (double)curr.Volume * ((double)curr.Close - (double)prev.Close);
        }

        return sum / LookbackPeriods;
    }
}

public static partial class ForceIndex
{
    /// <summary>
    /// Converts the quote provider to a Force Index hub.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A Force Index hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static ForceIndexHub ToForceIndexHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int lookbackPeriods = 2)
    {
        ArgumentNullException.ThrowIfNull(quoteProvider);
        return new(quoteProvider, lookbackPeriods);
    }

    /// <summary>
    /// Creates a Force Index hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="ForceIndexHub"/>.</returns>
    public static ForceIndexHub ToForceIndexHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 2)
    {
        ArgumentNullException.ThrowIfNull(quotes);
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToForceIndexHub(lookbackPeriods);
    }

}
