namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for creating CMF stream hubs.
/// </summary>
public class CmfHub : ChainProvider<IQuote, CmfResult>, ICmf
 {
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="CmfHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    internal CmfHub(
        IQuoteProvider<IQuote> provider,
        int lookbackPeriods)
        : base(provider)
    {
        Cmf.Validate(lookbackPeriods);

        LookbackPeriods = lookbackPeriods;
        hubName = $"CMF({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => Cache.Count == 0 ? hubName : $"{hubName}({Cache[0].Timestamp:d})";

    /// <inheritdoc/>
    protected override (CmfResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Calculate Money Flow Multiplier and Money Flow Volume using ADL formula
        double high = (double)item.High;
        double low = (double)item.Low;
        double close = (double)item.Close;
        double volume = (double)item.Volume;

        double mfm = high == low
            ? 0
            : (close - low - (high - close)) / (high - low);

        double mfv = mfm * volume;

        // Calculate CMF if we have enough data
        double? cmf = null;
        if (i >= LookbackPeriods - 1)
        {
            double sumMfv = 0;
            double sumVol = 0;

            // Sum over the lookback period
            for (int p = i + 1 - LookbackPeriods; p <= i; p++)
            {
                IQuote q = ProviderCache[p];
                double h = (double)q.High;
                double l = (double)q.Low;
                double c = (double)q.Close;
                double v = (double)q.Volume;

                double m = h == l ? 0 : (c - l - (h - c)) / (h - l);
                double mv = m * v;

                sumMfv += mv;
                sumVol += v;
            }

            double avgMfv = sumMfv / LookbackPeriods;
            double avgVol = sumVol / LookbackPeriods;

            if (avgVol != 0)
            {
                cmf = avgMfv / avgVol;
            }
        }

        // Create result
        CmfResult r = new(
            Timestamp: item.Timestamp,
            MoneyFlowMultiplier: mfm,
            MoneyFlowVolume: mfv,
            Cmf: cmf);

        return (r, i);
    }
}


public static partial class Cmf
{
    /// <summary>
    /// Converts the quote provider to a CMF hub.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods. Default is 20.</param>
    /// <returns>A CMF hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static CmfHub ToCmfHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int lookbackPeriods = 20)
         {
        ArgumentNullException.ThrowIfNull(quoteProvider);
        return new(quoteProvider, lookbackPeriods);
    }

    /// <summary>
    /// Creates a Cmf hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="CmfHub"/>.</returns>
    public static CmfHub ToCmfHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 20)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToCmfHub(lookbackPeriods);
    }

}
