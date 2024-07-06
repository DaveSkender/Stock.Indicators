namespace Skender.Stock.Indicators;

// ACCUMULATION/DISTRIBUTION LINE (STREAM HUB)

public class AdlHub<TQuote>
    : ChainHub<TQuote, AdlResult>
    where TQuote : struct, IQuote
{
    #region constructors

    public AdlHub(
        QuoteProvider<TQuote> provider)
        : this(provider, cache: new()) { }

    private AdlHub(
        QuoteProvider<TQuote> provider,
        StreamCache<AdlResult> cache)
        : base(provider, cache)
    {
        Reinitialize();
    }
    #endregion

    // METHODS

    public override string ToString()
    {
        if (StreamCache.Cache.Count == 0)
        {
            return "ADL";
        }

        AdlResult first = StreamCache.ReadCache[0];

        return $"ADL({first.Timestamp:d})";
    }

    public override void OnNextNew(TQuote newItem)
    {
        double prevAdl;
        QuoteD q = newItem.ToQuoteD();

        int i = Supplier.StreamCache.Position(newItem);

        if (i == 0)
        {
            prevAdl = 0;
        }
        else
        {
            AdlResult prev = StreamCache.ReadCache[i - 1];
            prevAdl = prev.Adl;
        }

        // calculate ADL
        AdlResult r = Adl.Increment(
            q.Timestamp, prevAdl,
            q.High, q.Low, q.Close, q.Volume);

        // save to cache
        Act act = StreamCache.Modify(Act.AddNew, r);

        // send to observers
        NotifyObservers(act, r);
    }
}
