namespace Skender.Stock.Indicators;

// ACCUMULATION/DISTRIBUTION LINE (STREAM)

public class Adl<TQuote>
    : AbstractQuoteInChainOut<TQuote, AdlResult>, IAdl
    where TQuote : struct, IQuote
{
    public Adl(IQuoteProvider<TQuote> provider)
        : base(provider)
    {
        // subscribe to quote provider
        Subscription = provider is null
            ? throw new ArgumentNullException(nameof(provider))
            : provider.Subscribe(this);
    }

    # region METHODS

    // string label
    public override string ToString()
        => Cache.Count == 0 ? "ADL" : $"ADL({Cache[0].Timestamp:d})";

    protected override void OnNextArrival(Act act, TQuote inbound)
    {
        int i;
        AdlResult r;

        // handle deletes
        if (act is Act.Delete)
        {
            i = Cache.FindIndex(inbound.Timestamp);

            // cache entry unexpectedly not found
            if (i == -1)
            {
                throw new InvalidOperationException(
                    "Matching cache entry not found.");
            }

            r = Cache[i];
        }

        // calculate incremental value
        else
        {
            i = Provider.FindIndex(inbound.Timestamp);

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
                : Cache[i - 1].Adl;

            // calculate ADL
            r = Adl.Increment(
                q.Timestamp, prevAdl,
                q.High, q.Low, q.Close, q.Volume);
        }

        // save to cache
        act = ModifyCache(act, r);

        // send to observers
        NotifyObservers(act, r);

        // cascade update forward values (recursively)
        if (act != Act.AddNew && i < ProviderCache.Count - 1)
        {
            int next = act == Act.Delete ? i : i + 1;
            TQuote value = ProviderCache[next];
            OnNextArrival(Act.Update, value);
        }
    }
    #endregion
}
