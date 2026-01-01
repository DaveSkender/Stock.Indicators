namespace Skender.Stock.Indicators;

/// <summary>
/// Provides functionality to calculate the Average True Range (ATR) for a series of quotes.
/// </summary>
public class AtrHub
    : ChainHub<IQuote, AtrResult>, IAtr
{
    internal AtrHub(IQuoteProvider<IQuote> provider,
        int lookbackPeriods)
        : base(provider)
    {
        Atr.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        Name = $"ATR({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

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
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An instance of <see cref="AtrHub"/>.</returns>
    public static AtrHub ToAtrHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int lookbackPeriods = 14)
             => new(quoteProvider, lookbackPeriods);
}
