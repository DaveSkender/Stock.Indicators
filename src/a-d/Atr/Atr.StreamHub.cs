namespace Skender.Stock.Indicators;

// AVERAGE TRUE RANGE (STREAM HUB)

#region hub interface

public interface IAtrHub
{
    int LookbackPeriods { get; }
}
#endregion

public class AtrHub<TIn> : QuoteObserver<TIn, AtrResult>,
    IReusableHub<TIn, AtrResult>, IAtrHub
    where TIn : IQuote
{
    #region constructors

    internal AtrHub(IQuoteProvider<TIn> provider,
        int lookbackPeriods)
        : base(provider)
    {
        Atr.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        Reinitialize();
    }
    #endregion

    public int LookbackPeriods { get; init; }

    // METHODS

    internal override void Add(TIn newIn, int? index)
    {
        int i = index ?? Provider.GetIndex(newIn, true);

        // skip incalculable periods
        if (i == 0)
        {
            Motify(new AtrResult(newIn.Timestamp), i);
            return;
        }

        AtrResult r;

        // re-initialize as average TR, if necessary
        if (Cache[i - 1].Atr is null && i >= LookbackPeriods)
        {
            double sumTr = 0;
            double tr = double.NaN;

            for (int p = i - LookbackPeriods + 1; p <= i; p++)
            {
                tr = Tr.Increment(
                    (double)Provider.Results[p].High,
                    (double)Provider.Results[p].Low,
                    (double)Provider.Results[p - 1].Close);

                sumTr += tr;
            }

            double atr = sumTr / LookbackPeriods;

            r = new AtrResult(
                newIn.Timestamp,
                tr,
                atr,
                atr / (double)newIn.Close * 100);
        }

        // calculate ATR (normally)
        else
        {
            r = Atr.Increment(
                LookbackPeriods,
                newIn,
                (double)Provider.Results[i - 1].Close,
                Cache[i - 1].Atr);
        }

        // save and send
        Motify(r, i);
    }

    public override string ToString() => $"ATR({LookbackPeriods})";
}
