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

    public Use(
        QuoteProvider<TQuote> provider,
        CandlePart candlePart)
        : this(provider, cache: new())
    {
        CandlePartSelection = candlePart;
    }

    private Use(
        QuoteProvider<TQuote> provider,
        StreamCache<Reusable> cache)
        : base(cache)
    {
        _cache = cache;
        _observer = new(this, this, provider);
    }

    public CandlePart CandlePartSelection { get; }


    // METHODS

    public override string ToString()
        => $"USE({Enum.GetName(typeof(CandlePart), CandlePartSelection)})";

    public void Unsubscribe() => _observer.Unsubscribe();

    public void OnNextArrival(Act act, TQuote inbound)
    {
        // candidate result
        Reusable result
            = inbound.ToReusable(CandlePartSelection);

        // save to cache
        _cache.ModifyCache(act, result);

        // send to observers
        NotifyObservers(act, result);
    }
}
