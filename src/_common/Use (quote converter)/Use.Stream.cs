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

    // string label
    public override string ToString()
        => $"USE({Enum.GetName(typeof(CandlePart), CandlePartSelection)})";

    // handle quote arrival
    public override void OnNext((Act act, TQuote quote) value)
    {
        // candidate result
        (DateTime d, double v) = value.quote.ToTuple(CandlePartSelection);
        UseResult r = new() { TickDate = d, Value = v };

        // save to cache
        CacheChainorPerAction(value.act, r, v);

        // send to observers
        NotifyObservers(value.act, r);
    }

    // delete cache between index values
    // usually called from inherited ClearCache(fromDate)
    internal override void ClearCache(int fromIndex, int toIndex)
    {
        // delete and deliver instruction,
        // in reverse order to prevent recompositions

        for (int i = toIndex; i >= fromIndex; i--)
        {
            UseResult r = Cache[i];
            Act act = CacheChainorPerAction(Act.Delete, r, double.NaN);
            NotifyObservers(act, r);
        }
    }
}
