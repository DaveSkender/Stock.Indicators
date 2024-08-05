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

public class AtrStopHub<TIn>
    : StreamHub<TIn, AtrStopResult>, IAtrStopHub
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
    
    // overridden to restore rebuild position/state (if needed)
    public override void OnNextAddition(TIn item, int? indexHint)
    {
        // add next value (standard)
        if (Cache.Count == 0 || item.Timestamp > Cache[^1].Timestamp)
        {
            base.OnNextAddition(item, indexHint);
            return;
        }

        // should only be rebuilt at this point
        int i = indexHint ?? ProviderCache.GetIndex(item, true);

        // rebuild from prior stop point
        if (i > LookbackPeriods)
        {
            AtrStopResult resetStop = Cache[i - 1];

            // reset prevailing direction and bands
            IsBullish = resetStop.AtrStop >= resetStop.SellStop;
            UpperBand = resetStop.BuyStop ?? default;
            LowerBand = resetStop.SellStop ?? default;

            // rebuild cache AFTER last sync point
            RebuildCache(resetStop.Timestamp);
        }

        // or full rebuild if no prior stop found
        else
        {
            IsBullish = default;
            UpperBand = default;
            LowerBand = default;
            RebuildCache();
        }
    }

    protected override (AtrStopResult result, int index)
        ToCandidate(TIn item, int? indexHint)
    {
        // reminder: should only process "new" instructions

        int i = indexHint ?? ProviderCache.GetIndex(item, true);

        // handle warmup periods
        if (i < LookbackPeriods)
        {
            return (new AtrStopResult(item.Timestamp), i);
        }

        QuoteD newQ = item.ToQuoteD();
        double prevClose = (double)ProviderCache[i - 1].Close;

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
                    (double)ProviderCache[p].High,
                    (double)ProviderCache[p].Low,
                    (double)ProviderCache[p - 1].Close);
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

            r = new AtrStopResult(
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

            r = new AtrStopResult(
                Timestamp: newQ.Timestamp,
                AtrStop: LowerBand,
                BuyStop: null,
                SellStop: LowerBand,
                Atr: atr);
        }

        return (r, i);
    }
}
