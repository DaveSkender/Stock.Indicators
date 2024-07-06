namespace Skender.Stock.Indicators;

// USE / QUOTE CONVERTER (STREAM HUB)

#region Hub interface
public interface IUseHub
{
    CandlePart CandlePartSelection { get; }
}
#endregion

public class Use<TQuote>
    : ChainHub<TQuote, Reusable>, IUseHub
    where TQuote : struct, IQuote
{
    #region constructors

    public Use(
        QuoteProvider<TQuote> provider,
        CandlePart candlePart)
        : this(provider, cache: new(), candlePart) { }

    private Use(
        QuoteProvider<TQuote> provider,
        StreamCache<Reusable> cache,
        CandlePart candlePart)
        : base(provider, cache)
    {
        CandlePartSelection = candlePart;

        Reinitialize();
    }
    #endregion

    public CandlePart CandlePartSelection { get; }

    // METHODS

    public override string ToString()
        => $"USE({Enum.GetName(CandlePartSelection)})";

    public override void OnNextNew(TQuote newItem)
    {
        // candidate result
        Reusable result
            = newItem.ToReusable(CandlePartSelection);

        // save to cache
        Act act = StreamCache.Modify(Act.AddNew, result);

        // send to observers
        NotifyObservers(act, result);
    }
}
