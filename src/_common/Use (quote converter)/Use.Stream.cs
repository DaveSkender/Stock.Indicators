namespace Skender.Stock.Indicators;

// USE (STREAMING)

public class Use<TQuote>
    : AbstractQuoteInChainOut<TQuote, Reusable>, IUse<TQuote>
    where TQuote : struct, IQuote
{
    #region CONSTRUCTORS

    public Use(
        IQuoteProvider<TQuote> provider,
        CandlePart candlePart) : base(provider)
    {
        CandlePartSelection = candlePart;

        // subscribe to quote provider
        Subscription = provider is null
            ? throw new ArgumentNullException(nameof(provider))
            : provider.Subscribe(this);
    }
    #endregion

    # region PROPERTIES

    public CandlePart CandlePartSelection { get; private set; }
    #endregion

    # region METHODS

    // string label
    public override string ToString()
        => $"USE({Enum.GetName(typeof(CandlePart), CandlePartSelection)})";

    // handle quote arrival
    protected override void OnNextArrival(Act act, TQuote inbound)
    {
        // candidate result
        Reusable result
            = inbound.ToReusable(CandlePartSelection);

        // save to cache
        ModifyCache(act, result);

        // send to observers
        NotifyObservers(act, result);
    }
    #endregion
}
