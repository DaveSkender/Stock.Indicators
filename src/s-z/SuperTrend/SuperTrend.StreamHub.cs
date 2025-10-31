namespace Skender.Stock.Indicators;

// SUPERTREND (STREAM HUB)

/// <summary>
/// Provides methods for calculating the SuperTrend using a stream hub.
/// </summary>
public class SuperTrendHub
    : StreamHub<IQuote, SuperTrendResult>, ISuperTrend
{
    #region constructors

    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="SuperTrendHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">The multiplier for the ATR.</param>
    internal SuperTrendHub(
        IQuoteProvider<IQuote> provider,
        int lookbackPeriods,
        double multiplier) : base(provider)
    {
        SuperTrend.Validate(lookbackPeriods, multiplier);

        LookbackPeriods = lookbackPeriods;
        Multiplier = multiplier;
        hubName = $"SUPERTREND({lookbackPeriods},{multiplier})";

        Reinitialize();
    }
    #endregion constructors

    /// <summary>
    /// Gets the number of periods to look back.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <summary>
    /// Gets the multiplier for the ATR.
    /// </summary>
    public double Multiplier { get; init; }

    /// <summary>
    /// prevailing direction and band thresholds
    /// </summary>
    private bool IsBullish { get; set; } = true;
    private double UpperBand { get; set; } = double.MaxValue;
    private double LowerBand { get; set; } = double.MinValue;
    private double PrevAtr { get; set; } = double.NaN;

    // METHODS

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (SuperTrendResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        // reminder: should only process "new" instructions

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // handle warmup periods
        if (i < LookbackPeriods)
        {
            return (new SuperTrendResult(item.Timestamp, null, null, null), i);
        }

        QuoteD newQ = item.ToQuoteD();
        double prevClose = (double)ProviderCache[i - 1].Close;

        // initialize direction on first evaluation
        if (i == LookbackPeriods)
        {
            double mid = (newQ.High + newQ.Low) / 2;
            IsBullish = newQ.Close >= mid;
        }

        // calculate ATR
        double atr;

        if (!double.IsNaN(PrevAtr))
        {
            atr = Atr.Increment(
                LookbackPeriods,
                newQ.High,
                newQ.Low,
                prevClose,
                PrevAtr);
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

        // store ATR for next iteration
        PrevAtr = atr;

        // calculate mid point
        double mid2 = (newQ.High + newQ.Low) / 2;

        // potential bands
        double upperEval = mid2 + (Multiplier * atr);
        double lowerEval = mid2 - (Multiplier * atr);

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

        // supertrend: based on direction
        SuperTrendResult r;

        // the upper band (bearish / short)
        if (newQ.Close <= (IsBullish ? LowerBand : UpperBand))
        {
            IsBullish = false;

            r = new SuperTrendResult(
                Timestamp: newQ.Timestamp,
                SuperTrend: (decimal?)UpperBand,
                UpperBand: (decimal?)UpperBand,
                LowerBand: null);
        }

        // the lower band (bullish / long)
        else
        {
            IsBullish = true;

            r = new SuperTrendResult(
                Timestamp: newQ.Timestamp,
                SuperTrend: (decimal?)LowerBand,
                UpperBand: null,
                LowerBand: (decimal?)LowerBand);
        }

        return (r, i);
    }

    /// <summary>
    /// Restores the prior SuperTrend state.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Reset all state
        IsBullish = true;
        UpperBand = double.MaxValue;
        LowerBand = double.MinValue;
        PrevAtr = double.NaN;

        if (timestamp <= DateTime.MinValue || ProviderCache.Count == 0)
        {
            return;
        }

        // Find the first index at or after timestamp
        int index = ProviderCache.IndexGte(timestamp);

        if (index <= 0)
        {
            // Rolling back before all data, keep cleared state
            return;
        }

        // We need to rebuild state up to the index before timestamp
        int targetIndex = index - 1;

        // Replay up to target to rebuild state
        for (int i = 0; i <= targetIndex; i++)
        {
            IQuote item = ProviderCache[i];

            // Skip warmup periods
            if (i < LookbackPeriods)
            {
                continue;
            }

            double high = (double)item.High;
            double low = (double)item.Low;
            double close = (double)item.Close;
            double prevClose = (double)ProviderCache[i - 1].Close;

            // Initialize direction on first evaluation
            if (i == LookbackPeriods)
            {
                double mid = (high + low) / 2;
                IsBullish = close >= mid;
            }

            // Calculate ATR
            double atr;
            if (!double.IsNaN(PrevAtr))
            {
                atr = Atr.Increment(
                    LookbackPeriods,
                    high,
                    low,
                    prevClose,
                    PrevAtr);
            }
            else
            {
                // Initialize ATR
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

            PrevAtr = atr;

            // Calculate mid point
            double mid2 = (high + low) / 2;

            // Potential bands
            double upperEval = mid2 + (Multiplier * atr);
            double lowerEval = mid2 - (Multiplier * atr);

            // New upper band
            if (upperEval < UpperBand || prevClose > UpperBand)
            {
                UpperBand = upperEval;
            }

            // New lower band
            if (lowerEval > LowerBand || prevClose < LowerBand)
            {
                LowerBand = lowerEval;
            }

            // Update direction
            if (close <= (IsBullish ? LowerBand : UpperBand))
            {
                IsBullish = false;
            }
            else
            {
                IsBullish = true;
            }
        }
    }
}


public static partial class SuperTrend
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SuperTrendHub"/> class.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">The multiplier for the ATR.</param>
    /// <returns>An instance of <see cref="SuperTrendHub"/>.</returns>
    public static SuperTrendHub ToSuperTrendHub(
       this IQuoteProvider<IQuote> quoteProvider,
       int lookbackPeriods = 10,
       double multiplier = 3)
           => new(quoteProvider, lookbackPeriods, multiplier);

    /// <summary>
    /// Creates a SuperTrend hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="SuperTrendHub"/>.</returns>
    public static SuperTrendHub ToSuperTrendHub(
        this IReadOnlyList<IQuote> quotes,
       int lookbackPeriods = 10,
       double multiplier = 3)
    {
        ArgumentNullException.ThrowIfNull(quotes);
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToSuperTrendHub(lookbackPeriods, multiplier);
    }

}
