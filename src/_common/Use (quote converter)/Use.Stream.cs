namespace Skender.Stock.Indicators;

// USE (STREAMING)

public class Use<TQuote> : QuoteObserver<TQuote, UseResult>
    where TQuote : IQuote, new()
{
    // constructor
    public Use(
        QuoteProvider<TQuote> provider,
        CandlePart candlePart) :
        base(provider)
    {
        CandlePartSelection = candlePart;

        Initialize();

        // subscribe to quote provider
        unsubscriber = provider is null
            ? throw new ArgumentNullException(nameof(provider))
            : provider.Subscribe(this);
    }

    // PROPERTIES

    private CandlePart CandlePartSelection { get; set; }


    // METHODS

    // handle quote arrival
    public override void OnNext((Act act, TQuote quote) value)
    {
        // candidate result
        (DateTime d, double v) = value.quote.ToTuple(CandlePartSelection);
        UseResult r = new() { Date = d, Value = v };

        // save to cache
        CacheChainorPerAction(value.act, r, v);

        // send to observers
        NotifyObservers(value.act, r);
    }

    // re/initialize cache, from provider
    public override void Initialize()
    {
        ResetCache();  // clears my cache (and notifies my observers)

        // replay from supplier cache
        List<TQuote> quotes = QuoteSupplier.Cache;

        for (int i = 0; i < quotes.Count; i++)
        {
            OnNext((Act.AddNew, quotes[i]));
        }
    }

    // delete cache, gracefully
    internal override void ResetCache()
    {
        // delete and deliver instruction,
        // in reverse order to prevent recompositions
        for (int i = Cache.Count - 1; i > 0; i--)
        {
            UseResult r = Cache[i];
            Act act = CacheChainorPerAction(Act.Delete, r, double.NaN);
            NotifyObservers((act, r.Date, r.Value));
        }
    }
}
