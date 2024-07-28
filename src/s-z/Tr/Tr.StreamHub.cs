namespace Skender.Stock.Indicators;

// TRUE RANGE (STREAM HUB)

public class TrHub<TIn> : QuoteObserver<TIn, TrResult>,
    IReusableHub<TIn, TrResult>
    where TIn : IQuote
{
    #region constructors

    internal TrHub(IQuoteProvider<TIn> provider)
        : base(provider)
    {
        Reinitialize();
    }
    #endregion

    // METHODS

    internal override void Add(Act act, TIn newIn, int? index)
    {
        int i = index ?? Provider.GetIndex(newIn, false);

        // skip first period
        if (i == 0)
        {
            Motify(act, new TrResult(newIn.Timestamp, null), i);
            return;
        }

        TIn prev = Provider.Results[i - 1];

        // candidate result
        TrResult r = new(
            newIn.Timestamp,
            Tr.Increment(
                (double)newIn.High,
                (double)newIn.Low,
                (double)prev.Close));

        // save and send
        Motify(act, r, i);
    }

    public override string ToString() => "TRUE RANGE";
}
