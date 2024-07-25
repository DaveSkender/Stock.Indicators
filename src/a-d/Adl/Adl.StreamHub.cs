namespace Skender.Stock.Indicators;

// ACCUMULATION/DISTRIBUTION LINE (STREAM HUB)

public class AdlHub<TIn> : QuoteObserver<TIn, AdlResult>,
    IReusableHub<TIn, AdlResult>
    where TIn : IQuote
{
    #region constructors

    internal AdlHub(IQuoteProvider<TIn> provider)
        : base(provider)
    {
        Reinitialize();
    }
    #endregion

    // METHODS

    internal override void Add(Act act, TIn newIn, int? index)
    {
        int i = index ?? Provider.GetIndex(newIn, false);

        // candidate result
        AdlResult r = Adl.Increment(
            newIn.Timestamp,
            newIn.High,
            newIn.Low,
            newIn.Close,
            newIn.Volume,
            i > 0 ? Cache[i - 1].Value : 0);

        // save and send
        Motify(act, r, i);
    }

    public override string ToString()
    {
        if (Cache.Count == 0)
        {
            return "ADL";
        }

        AdlResult first = Cache[0];

        return $"ADL({first.Timestamp:d})";
    }
}
