namespace Skender.Stock.Indicators;

// USE (STREAMING)

public class Use<TQuote>
    : AbstractQuoteInChainOut<TQuote, QuotePart>, IUse
    where TQuote : struct, IQuote
{
    // constructor
    public Use(
        IQuoteProvider<TQuote> provider,
        CandlePart candlePart) : base(provider)
    {
        CandlePartSelection = candlePart;

        RebuildCache();

        // subscribe to quote provider
        Subscription = provider is null
            ? throw new ArgumentNullException(nameof(provider))
            : provider.Subscribe(this);
    }

    // PROPERTIES

    public CandlePart CandlePartSelection { get; private set; }


    // METHODS

    // string label
    public override string ToString()
        => $"USE({Enum.GetName(typeof(CandlePart), CandlePartSelection)})";

    // handle quote arrival
    internal override void OnNextArrival(Act act, IQuote quote)
    {
        // candidate result
        (DateTime d, double v)
            = quote.ToTuple(CandlePartSelection);

        QuotePart result = new() { Timestamp = d, Value = v };

        // save to cache
        ModifyCache(act, result);

        // send to observers
        NotifyObservers(act, result);
    }
}
