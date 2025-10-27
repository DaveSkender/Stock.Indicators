namespace Skender.Stock.Indicators;

/// <summary>
/// Provides functionality to calculate the Average True Range (ATR) for a series of quotes.
/// </summary>
public class AtrHub
    : ChainProvider<IQuote, AtrResult>, IAtr
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="AtrHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods for ATR calculation.</param>
    internal AtrHub(IQuoteProvider<IQuote> provider,
        int lookbackPeriods)
        : base(provider)
    {
        Atr.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"ATR({lookbackPeriods})";

        Reinitialize();
    }

    /// <summary>
    /// Gets the number of lookback periods for ATR calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    // METHODS

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (AtrResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // skip incalculable periods
        if (i == 0)
        {
            return (new AtrResult(item.Timestamp), i);
        }

        AtrResult r;

        // re-initialize as average TR, if necessary
        if (Cache[i - 1].Atr is null && i >= LookbackPeriods)
        {
            double sumTr = 0;
            double tr = double.NaN;

            for (int p = i - LookbackPeriods + 1; p <= i; p++)
            {
                tr = Tr.Increment(
                    (double)ProviderCache[p].High,
                    (double)ProviderCache[p].Low,
                    (double)ProviderCache[p - 1].Close);

                sumTr += tr;
            }

            double atr = sumTr / LookbackPeriods;

            r = new AtrResult(
                item.Timestamp,
                tr,
                atr,
                atr / (double)item.Close * 100);
        }

        // calculate ATR (normally)
        else
        {
            r = Atr.Increment(
                LookbackPeriods,
                item,
                (double)ProviderCache[i - 1].Close,
                Cache[i - 1].Atr);
        }

        return (r, i);
    }
}


public static partial class Atr
{
    /// <summary>
    /// Converts the provided quote provider to an ATR hub with the specified lookback periods.
    /// </summary>
    /// <param name="quoteProvider">The quote provider to convert.</param>
    /// <param name="lookbackPeriods">The number of lookback periods for ATR calculation. Default is 14.</param>
    /// <returns>An instance of <see cref="AtrHub"/>.</returns>
    public static AtrHub ToAtrHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int lookbackPeriods = 14)
             => new(quoteProvider, lookbackPeriods);

    /// <summary>
    /// Creates a Atr hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="AtrHub"/>.</returns>
    public static AtrHub ToAtrHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToAtrHub(lookbackPeriods);
    }

}
