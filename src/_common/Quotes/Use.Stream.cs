namespace Skender.Stock.Indicators;

// USE (STREAMING)

// TODO: Get rid of TQuote modifier????????  We didn't need it before.
public class Use<TQuote> : ChainProvider, IChainProvider<BasicData>,
    IObserver<(Act act, TQuote quote)>
    where TQuote : IQuote, new()
{
    // fields
    private readonly IDisposable? unsubscriber;

    // constructor, with provider
    public Use(
        QuoteProvider<TQuote> provider,
        CandlePart candlePart)
    {
        QuoteSupplier = provider;
        CandlePartSelection = candlePart;

        Cache = [];
        LastArrival= new();

        Initialize();

        // subscribe to quote provider
        unsubscriber = provider != null
         ? provider.Subscribe(this)
         : throw new ArgumentNullException(nameof(provider));
    }

    // PROPERTIES

    // common

    public IEnumerable<BasicData> Results => Cache;

    public List<BasicData> Cache { get; set; }  // TODO: why can't these be internal set?

    public int OverflowCount { get ; set; }

    public BasicData LastArrival { get; set; }

    private QuoteProvider<TQuote> QuoteSupplier { get; set; }

    // unique

    private CandlePart CandlePartSelection { get; set; }


    // METHODS

    // re/initialize cache, from provider
    public void Initialize()
    {
        // clears cache (and notifies my observers)
        this.ResetCache(observers);

        // current provider cache
        List<TQuote> quotes = QuoteSupplier.Cache;

        // replay provider quotes
        for (int i = 0; i < quotes.Count; i++)
        {
            TQuote q = quotes[i];
            OnNext((Act.AddNew, q));
        }
    }

    // handle quote arrival
    public virtual void OnNext((Act act, TQuote quote) value)
    {
        // candidate result
        (DateTime d, double v) = value.quote.ToTuple(CandlePartSelection);
        BasicData r = new() { Date = d, Value = v };

        // save to cache
        this.CacheWithAction(value.act, r);

        // send to observers
        NotifyObservers(value.act, r);
    }

    public void OnCompleted() => Unsubscribe();

    public void Unsubscribe() => unsubscriber?.Dispose();
}
