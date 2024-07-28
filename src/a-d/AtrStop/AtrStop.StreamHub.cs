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

    internal AtrStopHub(
        IQuoteProvider<TIn> provider,
        int LookbackPeriods,
        double Multiplier,
        EndType EndType) : base(provider)
    {
        AtrStop.Validate(LookbackPeriods, Multiplier);

        this.LookbackPeriods = LookbackPeriods;
        this.Multiplier = Multiplier;
        this.EndType = EndType;

        Reinitialize();
    }
    #endregion

    public int LookbackPeriods { get; }
    public double Multiplier { get; }
    public EndType EndType { get; }

    // prevailing direction and band thresholds
    private bool IsBullish { get; set; } = true;
    private double UpperBand { get; set; } = double.MaxValue;
    private double LowerBand { get; set; } = double.MinValue;

    // METHODS

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
        // should only processes "new" instructions
        if (act != Act.AddNew)
        {
            throw new InvalidOperationException(
                "AtrStopHub should only receive new data.");
        }

        int i = index ?? Provider.GetIndex(newIn, false);

        double? atr = null;
        decimal? atrStop = null;
        decimal? buyStop = null;
        decimal? sellStop = null;

        if (i >= LookbackPeriods)
        {
            QuoteD newQ = newIn.ToQuoteD();
            QuoteD prevQ = Provider.Results[i - 1].ToQuoteD();

            // calculate ATR
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
            double? upperEval;
            double? lowerEval;

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

            // initialize values on first eval pass
            if (i == LookbackPeriods)
            {
                IsBullish = newQ.Close >= prevQ.Close;
            }

            // new upper band: can only go down, or reverse
            if (upperEval < UpperBand || prevQ.Close > UpperBand)
            {
                UpperBand = (double)upperEval;
            }

            // new lower band: can only go up, or reverse
            if (lowerEval > LowerBand || prevQ.Close < LowerBand)
            {
                LowerBand = (double)lowerEval;
            }

            // trailing stop: based on direction, can be either:
            // the upper band (buy-to-stop) or
            // the lower band (sell-to-stop)
            if (newQ.Close <= (IsBullish ? LowerBand : UpperBand))
            {
                atrStop = (decimal?)UpperBand;
                buyStop = (decimal?)UpperBand;
                IsBullish = false;
            }
            else
            {
                atrStop = (decimal?)LowerBand;
                sellStop = (decimal?)LowerBand;
                IsBullish = true;
            }
        }

        AtrStopResult r = new(
            Timestamp: newIn.Timestamp,
            AtrStop: atrStop,
            BuyStop: buyStop,
            SellStop: sellStop,
            Atr: atr);

        // save and send
        Motify(act, r, null);
    }

    public override string ToString()
        => $"ATR-STOP({LookbackPeriods},{Multiplier},{EndType})";
}
