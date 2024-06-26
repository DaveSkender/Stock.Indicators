namespace Skender.Stock.Indicators;

// ACCUMULATION/DISTRIBUTION LINE (STREAM)

public partial class Adl<TQuote> : AbstractQuoteInChainOut<TQuote, AdlResult>, IAdl
    where TQuote : struct, IQuote
{
    public Adl(IQuoteProvider<TQuote> provider)
        : base(provider)
    {

        // subscribe to quote provider
        Subscription = provider is null
            ? throw new ArgumentNullException(nameof(provider))
            : provider.Subscribe(this);
    }

    // string label
    public override string ToString()
        => Cache.Count == 0 ? "ADL" : $"ADL({Cache[0].Timestamp:d})";

    internal override void OnNextArrival(Act act, IQuote quote)
        => throw new NotImplementedException();
}
