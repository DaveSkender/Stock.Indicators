namespace Skender.Stock.Indicators;

// USE (STREAMING)

// TODO: Get rid of TQuote modifier????????  We didn't need it before.
public class Use<TQuote> : ChainProvider<BasicData>,
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
        Initialize();

        // subscribe to quote provider
        unsubscriber = provider != null
         ? provider.Subscribe(this)
         : throw new ArgumentNullException(nameof(provider));
    }

    // constructor, needs provider
    public Use(CandlePart candlePart)
    {
        QuoteProvider<TQuote> provider = new();

        QuoteSupplier = provider;
        CandlePartSelection = candlePart;
        Initialize();

        // subscribe to quote provider
        unsubscriber = provider.Subscribe(this);
    }


    private QuoteProvider<TQuote> QuoteSupplier { get; set; }

    private CandlePart CandlePartSelection { get; set; }

    // METHODS

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

    // re/initialize my cache, from provider cache
    private void Initialize()
    {
        ResetCache();  // clears my cache (and notifies my observers)

        // current provider cache
        List<TQuote> quotes = QuoteSupplier.Cache;

        // replay provider quotes
        for (int i = 0; i < quotes.Count; i++)
        {
            TQuote q = quotes[i];
            OnNext((Act.AddNew, q));
        }
    }

    // add one
    public Act Add(TQuote quote)
    {
        (DateTime date, double value) = quote.ToTuple(CandlePart.Close);

        // candidate result
        BasicData r = new()
        {
            Date = date,
            Value = value
        };

        // save to cache
        Act act = this.CacheWithAnalysis(r);

        // send to observers
        NotifyObservers(act, r);

        return act;
    }

    // add many
    public void Add(IEnumerable<TQuote> quotes)
    {
        List<TQuote> added = quotes
            .ToSortedList();

        for (int i = 0; i < added.Count; i++)
        {
            Add(added[i]);
        }
    }
}
