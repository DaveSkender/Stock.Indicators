namespace Skender.Stock.Indicators;

// ACCUMULATION/DISTRIBUTION LINE (STREAM)

public class Adl<TQuote> : AbstractQuoteInChainOut<TQuote, AdlResult>, IAdl
    where TQuote : struct, IQuote
{
    #region CONSTRUCTORS

    public Adl(IQuoteProvider<TQuote> provider)
        : base(provider)
    {

        // subscribe to quote provider
        Subscription = provider is null
            ? throw new ArgumentNullException(nameof(provider))
            : provider.Subscribe(this);
    }
    #endregion

    # region METHODS

    // string label
    public override string ToString()
        => Cache.Count == 0 ? "ADL" : $"ADL({Cache[0].Timestamp:d})";

    protected override void OnNextArrival(Act act, TQuote inbound)
        => throw new NotImplementedException();
    #endregion
}
