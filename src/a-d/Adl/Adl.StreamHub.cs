namespace Skender.Stock.Indicators;

// ACCUMULATION/DISTRIBUTION LINE (STREAM HUB)

public class AdlHub<TIn>
    : ChainProvider<TIn, AdlResult>
    where TIn : IQuote
{
    #region constructors

    internal AdlHub(IQuoteProvider<TIn> provider)
        : base(provider)
    {
        Reinitialize();
    }
    #endregion

    // METHODS

    protected override (AdlResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.GetIndex(item, true);

        // candidate result
        AdlResult r = Adl.Increment(
            item.Timestamp,
            item.High,
            item.Low,
            item.Close,
            item.Volume,
            i > 0 ? Cache[i - 1].Value : 0);

        return (r, i);
    }

    public override string ToString()
    {
        if (Cache.Count == 0)
        {
            return "ADL";
        }

        return $"ADL({Cache[0].Timestamp:d})";
    }
}
