namespace Skender.Stock.Indicators;

// ACCUMULATION/DISTRIBUTION LINE (STREAM HUB)

public class AdlHub<TIn> : QuoteObserver<TIn, AdlResult>,
    IReusableHub<TIn, AdlResult>
    where TIn : IQuote
{
    #region constructors

    public AdlHub(IQuoteProvider<TIn> provider)
        : base(provider)
    {
        Reinitialize();
    }
    #endregion

    // METHODS

    internal override void Add(Act act, TIn newIn, int? index)
    {
        if (newIn is null)
        {
            throw new ArgumentNullException(nameof(newIn));
        }

        double prevAdl;
        QuoteD q = newIn.ToQuoteD();

        int i = index ?? Supplier.GetIndex(newIn, false);

        if (i == 0)
        {
            prevAdl = 0;
        }
        else
        {
            AdlResult prev = Cache[i - 1];
            prevAdl = prev.Adl;
        }

        // calculate ADL
        AdlResult r = Adl.Increment(
            q.Timestamp, prevAdl,
            q.High, q.Low, q.Close, q.Volume);

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
