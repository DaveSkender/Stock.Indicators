namespace Skender.Stock.Indicators;

// USE (STREAMING)

#region Hub interface
public interface IUseHub
{
    CandlePart CandlePartSelection { get; }
}
#endregion

public class Use<TQuote>
    : ChainProvider<Reusable>, IStreamHub<TQuote, Reusable>, IUseHub
    where TQuote : struct, IQuote
{
    private readonly StreamCache<Reusable> _cache;
    private readonly StreamObserver<TQuote, Reusable> _observer;

    #region constructors

    public Use(
        QuoteProvider<TQuote> provider,
        CandlePart candlePart)
        : this(provider, cache: new(), candlePart) { }

    private Use(
        QuoteProvider<TQuote> provider,
        StreamCache<Reusable> cache,
        CandlePart candlePart)
        : base(cache)
    {
        CandlePartSelection = candlePart;

        _cache = cache;
        _observer = new(this, cache, this, provider);
    }
    #endregion

    public CandlePart CandlePartSelection { get; }


    // METHODS

    public override string ToString()
        => $"USE({Enum.GetName(CandlePartSelection)})";

    public void Unsubscribe() => _observer.Unsubscribe();

    public void OnNextNew(TQuote newItem)
    {
        // candidate result
        Reusable result
            = newItem.ToReusable(CandlePartSelection);

        // save to cache
        Act act = _cache.Modify(Act.AddNew, result);

        // send to observers
        NotifyObservers(act, result);
    }
}
