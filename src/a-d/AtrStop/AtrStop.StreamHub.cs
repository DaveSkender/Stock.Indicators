namespace Skender.Stock.Indicators;

// ATR TRAILING STOP (STREAM HUB)

/// <summary>
/// Provides methods for calculating the ATR Trailing Stop using a stream hub.
/// </summary>
public static partial class AtrStop
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AtrStopHub{TIn}"/> class.
    /// </summary>
    /// <typeparam name="TIn">The type of the input quote.</typeparam>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back. Default is 21.</param>
    /// <param name="multiplier">The multiplier for the ATR. Default is 3.</param>
    /// <param name="endType">The type of price to use for the calculation. Default is <see cref="EndType.Close"/>.</param>
    /// <returns>An instance of <see cref="AtrStopHub{TIn}"/>.</returns>
    public static AtrStopHub<TIn> ToAtrStop<TIn>(
       this IQuoteProvider<TIn> quoteProvider,
       int lookbackPeriods = 21,
       double multiplier = 3,
       EndType endType = EndType.Close)
       where TIn : IQuote
       => new(quoteProvider, lookbackPeriods, multiplier, endType);
}

/// <summary>
/// Represents a stream hub for calculating the ATR Trailing Stop.
/// </summary>
/// <typeparam name="TIn">The type of the input quote.</typeparam>
public class AtrStopHub<TIn>
    : StreamHub<TIn, AtrStopResult>, IAtrStop
    where TIn : IQuote
{
    #region constructors

    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="AtrStopHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back.</param>
    /// <param name="multiplier">The multiplier for the ATR.</param>
    /// <param name="endType">The type of price to use for the calculation.</param>
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

    /// <summary>
    /// Gets the number of periods to look back.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <summary>
    /// Gets the multiplier for the ATR.
    /// </summary>
    public double Multiplier { get; init; }

    /// <summary>
    /// Gets the type of price to use for the calculation.
    /// </summary>
    public EndType EndType { get; init; }

    // prevailing direction and band thresholds
    private bool IsBullish { get; set; } = true;
    private double UpperBand { get; set; } = double.MaxValue;
    private double LowerBand { get; set; } = double.MinValue;

    // METHODS

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (AtrStopResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        // reminder: should only process "new" instructions

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

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

    /// <summary>
    /// Restores the prior ATR Stop state.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        int i = ProviderCache.IndexGte(timestamp);

        // restore prior stop point
        if (i > LookbackPeriods)
        {
            AtrStopResult resetStop = Cache[i - 1];

            // prevailing direction and bands
            IsBullish = resetStop.AtrStop >= resetStop.SellStop;
            UpperBand = resetStop.BuyStop ?? default;
            LowerBand = resetStop.SellStop ?? default;
        }

        // or reset if no prior stop found
        else
        {
            IsBullish = default;
            UpperBand = default;
            LowerBand = default;
        }
    }
}
