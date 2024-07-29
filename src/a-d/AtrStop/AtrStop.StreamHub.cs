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

    public int LookbackPeriods { get; init; }
    public double Multiplier { get; init; }
    public EndType EndType { get; init; }

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

        int i = index ?? Provider.GetIndex(item, false);

        // reset to the prior stop points
        if (i > LookbackPeriods)
        {
            AtrStopResult resetStop = Cache[i - 1];

            // reset prevailing direction and bands
            IsBullish = resetStop.AtrStop >= resetStop.SellStop;
            UpperBand = resetStop.BuyStop ?? default;
            LowerBand = resetStop.SellStop ?? default;

            // rebuild cache AFTER last sync point
            RebuildCache(i, i);
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
        double prevClose = (double)Provider.Results[i - 1].Close;

        // initialize direction on first evaluation
        if (i == LookbackPeriods)
        {
            IsBullish = newQ.Close >= prevClose;
        }

        // calculate ATR
        double atr;

        if (Cache[i - 1].Atr is not null)
        {
            atr = Atr.Increment(
                LookbackPeriods,
                newQ.High,
                newQ.Low,
                prevClose,
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
        if (upperEval < UpperBand || prevClose > UpperBand)
        {
            UpperBand = upperEval;
        }

        // new lower band: can only go up, or reverse
        if (lowerEval > LowerBand || prevClose < LowerBand)
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
                AtrStop: UpperBand,
                BuyStop: UpperBand,
                SellStop: null,
                Atr: atr);
        }

        // the lower band (long / sell-to-stop)
        else
        {
            IsBullish = true;

            r = new(
                Timestamp: newQ.Timestamp,
                AtrStop: LowerBand,
                BuyStop: null,
                SellStop: LowerBand,
                Atr: atr);
        }

        // save and send
        Motify(act, r, null);
    }

    public override string ToString()
        => $"ATR-STOP({LookbackPeriods},{Multiplier},{EndType.ToString().ToUpperInvariant()})";
}
