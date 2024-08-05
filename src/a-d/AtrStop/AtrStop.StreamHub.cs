namespace Skender.Stock.Indicators;

// ATR TRAILING STOP (STREAM HUB)

#region hub interface

public interface IAtrStopHub
{
    int LookbackPeriods { get; }
    double Multiplier { get; }
    EndType EndType { get; }
}
#endregion

public class AtrStopHub<TIn> : QuoteObserver<TIn, AtrStopResult>,
    IResultHub<TIn, AtrStopResult>, IAtrStopHub
    where TIn : IQuote
{
    #region constructors

    private readonly string hubName;

    internal AtrStopHub(
        IQuoteProvider<TIn> provider,
        int lookbackPeriods,
        double multiplier,
        EndType endType) : base(provider)
    {
        AtrStop.Validate(lookbackPeriods, multiplier);

        LookbackPeriods = lookbackPeriods;
        Multiplier = multiplier;
        EndType = endType;
        hubName = $"ATR-STOP({lookbackPeriods},{multiplier},{endType.ToString().ToUpperInvariant()})";

        Reinitialize();
    }
    #endregion

    public int LookbackPeriods { get; init; }
    public double Multiplier { get; init; }
    public EndType EndType { get; init; }

    // prevailing direction and band thresholds
    private bool IsBullish { get; set; } = true;
    private double UpperBand { get; set; } = double.MaxValue;
    private double LowerBand { get; set; } = double.MinValue;

    // METHODS

    public override string ToString() => hubName;

    // overridden to handle non-standard arrival scenarios
    public override void OnNext((Act, TIn, int?) value)
    {
        (Act act, TIn item, int? index) = value;

        // add next value
        if (act is Act.AddNew)
        {
            Add(act, item, index);
            return;
        }

        // find last synchronized band position (before deviance)
        int lastSyncIndex = Cache.FindLastIndex(
            x => x.Timestamp < item.Timestamp
              && x.AtrStop == (IsBullish ? x.SellStop : x.BuyStop));

        // rebuild from last know reversal point
        if (lastSyncIndex > LookbackPeriods)
        {
            AtrStopResult lastSyncPoint = Cache[lastSyncIndex];

            // reset prevailing direction and bands
            IsBullish = lastSyncPoint.AtrStop == lastSyncPoint.SellStop;
            UpperBand = (double?)lastSyncPoint.BuyStop ?? default;
            LowerBand = (double?)lastSyncPoint.SellStop ?? default;

            // rebuild cache AFTER last sync point
            RebuildCache(lastSyncIndex + 1, lastSyncIndex + 1);
        }

        // full rebuild if no prior reversal
        else
        {
            IsBullish = default;
            UpperBand = default;
            LowerBand = default;
            RebuildCache();
        }
    }

    internal override void Add(Act act, TIn newIn, int? index)
    {
        // reminder: should only processes "new" instructions

        int i = index ?? Provider.GetIndex(newIn, false);

        // handle warmup periods
        if (i < LookbackPeriods)
        {
            Motify(act, new(newIn.Timestamp), null);
            return;
        }

        QuoteD newQ = newIn.ToQuoteD();
        QuoteD prevQ = Provider.Results[i - 1].ToQuoteD();

        // initialize direction on first evaluation
        if (i == LookbackPeriods)
        {
            IsBullish = newQ.Close >= prevQ.Close;
        }

        // calculate ATR
        double atr;

        if (Cache[i - 1].Atr is not null)
        {
            atr = Atr.Increment(
                LookbackPeriods,
                newQ.High,
                newQ.Low,
                prevQ.Close,
                Cache[i - 1].Atr ?? double.NaN);
        }

        // initialize ATR
        else
        {
            double sumTr = 0;

            for (int p = i - LookbackPeriods + 1; p <= i; p++)
            {
                sumTr += Tr.Increment(
                    (double)Provider.Results[p].High,
                    (double)Provider.Results[p].Low,
                    (double)Provider.Results[p - 1].Close);
            }

            atr = sumTr / LookbackPeriods;
        }

        // evaluate bands
        double upperEval;
        double lowerEval;

        // potential bands for CLOSE
        if (EndType == EndType.Close)
        {
            upperEval = newQ.Close + (Multiplier * atr);
            lowerEval = newQ.Close - (Multiplier * atr);
        }

        // potential bands for HIGH/LOW
        else
        {
            upperEval = newQ.High + (Multiplier * atr);
            lowerEval = newQ.Low - (Multiplier * atr);
        }

        // new upper band: can only go down, or reverse
        if (upperEval < UpperBand || prevQ.Close > UpperBand)
        {
            UpperBand = upperEval;
        }

        // new lower band: can only go up, or reverse
        if (lowerEval > LowerBand || prevQ.Close < LowerBand)
        {
            LowerBand = lowerEval;
        }

        // trailing stop: based on direction

        AtrStopResult r;

        // the upper band (short / buy-to-stop)
        if (newQ.Close <= (IsBullish ? LowerBand : UpperBand))
        {
            IsBullish = false;

            r = new(
                Timestamp: newQ.Timestamp,
                AtrStop: (decimal)UpperBand,
                BuyStop: (decimal)UpperBand,
                SellStop: null,
                Atr: atr);
        }

        // the lower band (long / sell-to-stop)
        else
        {
            IsBullish = true;

            r = new(
                Timestamp: newQ.Timestamp,
                AtrStop: (decimal)LowerBand,
                BuyStop: null,
                SellStop: (decimal)LowerBand,
                Atr: atr);
        }

        // save and send
        Motify(act, r, null);
    }
}
