namespace Skender.Stock.Indicators;

public static partial class Atr
{
    /// <summary>
    /// Converts the provided quote provider to an ATR hub with the specified lookback periods.
    /// </summary>
    /// <typeparam name="TIn">The type of the input quote.</typeparam>
    /// <param name="quoteProvider">The quote provider to convert.</param>
    /// <param name="lookbackPeriods">The number of lookback periods for ATR calculation. Default is 14.</param>
    /// <returns>An instance of <see cref="AtrHub{TIn}"/>.</returns>
    [Stream("ATR", "Average True Range (ATR)", Category.PriceCharacteristic, ChartType.Oscillator)]
    public static AtrHub<TIn> ToAtr<TIn>(
        this IQuoteProvider<TIn> quoteProvider,
        [ParamNum<int>("Lookback Periods", 2, 250, 14)]
        int lookbackPeriods = 14)
        where TIn : IQuote
        => new(quoteProvider, lookbackPeriods);
}

/// <summary>
/// Represents a hub for calculating the Average True Range (ATR) indicator.
/// </summary>
/// <typeparam name="TIn">The type of the input quote.</typeparam>
public class AtrHub<TIn>
    : ChainProvider<TIn, AtrResult>, IAtr
    where TIn : IQuote
{
    #region constructors

    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="AtrHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods for ATR calculation.</param>
    internal AtrHub(IQuoteProvider<TIn> provider,
        int lookbackPeriods)
        : base(provider)
    {
        Atr.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"ATR({lookbackPeriods})";

        Reinitialize();
    }
    #endregion

    /// <summary>
    /// Gets the number of lookback periods for ATR calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    // METHODS

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (AtrResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
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
