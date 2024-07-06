namespace Skender.Stock.Indicators;

// ACCUMULATION/DISTRIBUTION LINE (STREAM)

public class AdlHub<TQuote>
    : ChainProvider<AdlResult>, IStreamHub<TQuote, AdlResult>
    where TQuote : struct, IQuote
{
    private readonly StreamCache<AdlResult> _cache;
    private readonly StreamObserver<TQuote, AdlResult> _observer;
    private readonly QuoteProvider<TQuote> _supplier;

    #region constructors

    public AdlHub(
        QuoteProvider<TQuote> provider)
        : this(provider, cache: new()) { }

    private AdlHub(
        QuoteProvider<TQuote> provider,
        StreamCache<AdlResult> cache) : base(cache)
    {
        _cache = cache;
        _supplier = provider;
        _observer = new(this, this, provider);
    }
    #endregion

    // METHODS

    public override string ToString()
    {
        if (_cache.Cache.Count == 0)
        {
            return "ADL";
        }

        AdlResult first = _cache.ReadCache[0];

        return $"ADL({first.Timestamp:d})";
    }

    public void Unsubscribe() => _observer.Unsubscribe();

    public void OnNextNew(TQuote newItem)
    {
        double prevAdl;
        QuoteD q = newItem.ToQuoteD();

        int i = _supplier.Position(newItem);

        if (i == 0)
        {
            prevAdl = 0;
        }
        else
        {
            AdlResult prev = _cache.ReadCache[i - 1];
            prevAdl = prev.Adl;
        }

        // calculate ADL
        AdlResult r = Adl.Increment(
            q.Timestamp, prevAdl,
            q.High, q.Low, q.Close, q.Volume);

        // save to cache
        Act act = _cache.Modify(Act.AddNew, r);

        // send to observers
        NotifyObservers(act, r);
    }
}
