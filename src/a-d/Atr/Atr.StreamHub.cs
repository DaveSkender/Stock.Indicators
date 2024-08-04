namespace Skender.Stock.Indicators;

// AVERAGE TRUE RANGE (STREAM HUB)

#region hub interface

public interface IAtrHub
{
    int LookbackPeriods { get; }
}
#endregion

public class AtrHub<TIn>
    : ChainProvider<TIn, AtrResult>, IAtrHub
    where TIn : IQuote
{
    #region constructors

    internal AtrHub(IQuoteProvider<TIn> provider,
        int lookbackPeriods)
        : base(provider)
    {
        Atr.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        Reinitialize();
    }
    #endregion

    public int LookbackPeriods { get; init; }

    // METHODS

    protected override (AtrResult result, int? index)
        ToCandidate(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.GetIndex(item, true);

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

    public override string ToString() => $"ATR({LookbackPeriods})";
}
