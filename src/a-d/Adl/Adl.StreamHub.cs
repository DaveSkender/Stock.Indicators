namespace Skender.Stock.Indicators;

// ACCUMULATION/DISTRIBUTION LINE (STREAM)

public class AdlHub<TQuote>
    : ChainProvider<AdlResult>, IStreamHub<TQuote, AdlResult>
    where TQuote : struct, IQuote
{
    private readonly StreamCache<AdlResult> _cache;
    private readonly StreamObserver<TQuote, AdlResult> _observer;
    private readonly QuoteProvider<TQuote> _supplier;

    public AdlHub(
        QuoteProvider<TQuote> provider)
        : this(provider, cache: new()) { }

    private AdlHub(
        QuoteProvider<TQuote> provider,
        StreamCache<AdlResult> cache) : base(cache)
    {
        _cache = cache;
        _observer = new(this, this, provider);
        _supplier = provider;
    }


    // METHODS

    public override string ToString()
        => _cache.Cache.Count == 0
            ? "ADL"
            : $"ADL({_cache.Cache[0].Timestamp:d})";

    public void Unsubscribe() => _observer.Unsubscribe();

    public void OnNextArrival(Act act, TQuote inbound)
    {
        int i;
        AdlResult r;

        // handle deletes
        if (act is Act.Delete)
        {
            i = _cache.Cache.FindIndex(c => c.Timestamp == inbound.Timestamp);

            // cache entry unexpectedly not found
            if (i == -1)
            {
                throw new InvalidOperationException(
                    "Matching cache entry not found.");
            }

            r = _cache.Cache[i];
        }

        // calculate incremental value
        else
        {
            i = _supplier.Cache.FindIndex(c => c.Timestamp == inbound.Timestamp);

            // source unexpectedly not found
            if (i == -1)
            {
                throw new InvalidOperationException(
                    "Matching source history not found.");
            }

            QuoteD q = inbound.ToQuoteD();

            double prevAdl
                = i == 0
                ? 0
                : _cache.Cache[i - 1].Adl;

            // calculate ADL
            r = Adl.Increment(
                q.Timestamp, prevAdl,
                q.High, q.Low, q.Close, q.Volume);
        }

        // save to cache
        act = _cache.ModifyCache(act, r);

        // send to observers
        NotifyObservers(act, r);

        // cascade update forward values (recursively)
        if (act != Act.AddNew && i < _supplier.Cache.Count - 1)
        {
            int next = act == Act.Delete ? i : i + 1;
            TQuote value = _supplier.Cache[next];
            OnNextArrival(Act.Update, value);
        }
    }
}
