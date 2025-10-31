namespace Skender.Stock.Indicators;

// VOLATILITY STOP (STREAM HUB)

/// <summary>
/// Provides methods for calculating the Volatility Stop using a stream hub.
/// </summary>
public class VolatilityStopHub
    : StreamHub<IQuote, VolatilityStopResult>, IVolatilityStop
{
    #region constructors

    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="VolatilityStopHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">The multiplier for the Average True Range.</param>
    internal VolatilityStopHub(
        IQuoteProvider<IQuote> provider,
        int lookbackPeriods,
        double multiplier) : base(provider)
    {
        VolatilityStop.Validate(lookbackPeriods, multiplier);

        LookbackPeriods = lookbackPeriods;
        Multiplier = multiplier;
        hubName = $"VOLATILITY-STOP({lookbackPeriods},{multiplier})";

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
    /// Significant close value
    /// </summary>
    private double Sic { get; set; }

    /// <summary>
    /// Current trend direction (true = long, false = short)
    /// </summary>
    private bool IsLong { get; set; }

    /// <summary>
    /// Track if first stop has been found
    /// </summary>
    private bool FirstStopFound { get; set; }

    /// <summary>
    /// Track if nullification has been performed
    /// </summary>
    private bool NullificationDone { get; set; }

    /// <summary>
    /// Previous ATR value for incremental calculation
    /// </summary>
    private double? PrevAtr { get; set; }

    // METHODS

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (VolatilityStopResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // During initialization period (first lookbackPeriods quotes)
        if (i < LookbackPeriods)
        {
            // On the last initialization period, determine trend direction
            if (i == LookbackPeriods - 1)
            {
                Sic = (double)ProviderCache[0].Close;
                double currentClose = (double)item.Close;
                IsLong = currentClose > Sic;

                // Update sic for all initialization periods based on determined trend
                for (int j = 0; j <= i; j++)
                {
                    double closePriceJ = (double)ProviderCache[j].Close;
                    Sic = IsLong ? Math.Max(Sic, closePriceJ) : Math.Min(Sic, closePriceJ);
                }
            }

            return (new VolatilityStopResult(item.Timestamp), i);
        }

        // Calculate ATR
        double close = (double)item.Close;
        double prevClose = (double)ProviderCache[i - 1].Close;
        double atr;

        if (PrevAtr.HasValue)
        {
            // Incremental ATR calculation
            atr = Atr.Increment(
                LookbackPeriods,
                (double)item.High,
                (double)item.Low,
                prevClose,
                PrevAtr.Value);
        }
        else
        {
            // Initialize ATR (first time after warmup)
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

        // Store ATR for next iteration
        PrevAtr = atr;

        // Calculate SAR
        double arc = atr * Multiplier;
        double sar = IsLong ? Sic - arc : Sic + arc;

        // Determine bands
        double? lowerBand = null;
        double? upperBand = null;

        if (IsLong)
        {
            lowerBand = sar;
        }
        else
        {
            upperBand = sar;
        }

        // Evaluate stop and reverse
        bool? isStop;

        if ((IsLong && close < sar) || (!IsLong && close > sar))
        {
            isStop = true;
            Sic = close;
            IsLong = !IsLong;
        }
        else
        {
            isStop = false;
            // Update significant close
            Sic = IsLong ? Math.Max(Sic, close) : Math.Min(Sic, close);
        }

        VolatilityStopResult result = new(
            Timestamp: item.Timestamp,
            Sar: sar,
            IsStop: isStop,
            UpperBand: upperBand,
            LowerBand: lowerBand);

        // If this is the first stop, mark it for nullification
        if (isStop == true && !FirstStopFound)
        {
            FirstStopFound = true;

            // Only nullify once per rebuild
            if (!NullificationDone)
            {
                NullificationDone = true;
                NullifyResultsBeforeFirstStop(i);

                // Also nullify the current result
                result = result with {
                    Sar = null,
                    UpperBand = null,
                    LowerBand = null,
                    IsStop = null
                };
            }
        }

        return (result, i);
    }

    /// <summary>
    /// Nullifies all results from lookbackPeriods to current position (inclusive).
    /// This is called when the first stop is detected.
    /// </summary>
    /// <param name="stopIndex">The index of the first stop.</param>
    private void NullifyResultsBeforeFirstStop(int stopIndex)
    {
        // Nullify all results from lookbackPeriods to stopIndex (inclusive of the stop)
        for (int idx = LookbackPeriods; idx <= stopIndex && idx < Cache.Count; idx++)
        {
            VolatilityStopResult existing = Cache[idx];
            Cache[idx] = existing with {
                Sar = null,
                UpperBand = null,
                LowerBand = null,
                IsStop = null
            };
        }
    }

    /// <summary>
    /// Restores the Volatility Stop state up to the specified timestamp.
    /// </summary>
    /// <inheritdoc/>
    /// <summary>
    /// Restores the Volatility Stop state up to the specified timestamp.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        int targetIndex = ProviderCache.IndexGte(timestamp);

        // Reset all state
        Sic = 0;
        IsLong = false;
        FirstStopFound = false;
        NullificationDone = false;
        PrevAtr = null;

        if (targetIndex <= LookbackPeriods)
        {
            return;
        }

        // Replay the calculation from the beginning up to targetIndex - 1
        int restoreIndex = targetIndex - 1;

        // Initialize trend (same as in ToIndicator)
        Sic = (double)ProviderCache[0].Close;
        double lastInitClose = (double)ProviderCache[LookbackPeriods - 1].Close;
        IsLong = lastInitClose > Sic;

        for (int j = 0; j < LookbackPeriods; j++)
        {
            double close = (double)ProviderCache[j].Close;
            Sic = IsLong ? Math.Max(Sic, close) : Math.Min(Sic, close);
        }

        // Replay calculations from LookbackPeriods to restoreIndex
        for (int j = LookbackPeriods; j <= restoreIndex; j++)
        {
            double close = (double)ProviderCache[j].Close;
            double prevClose = (double)ProviderCache[j - 1].Close;

            // Calculate ATR
            double atr;
            if (PrevAtr.HasValue)
            {
                atr = Atr.Increment(
                    LookbackPeriods,
                    (double)ProviderCache[j].High,
                    (double)ProviderCache[j].Low,
                    prevClose,
                    PrevAtr.Value);
            }
            else
            {
                double sumTr = 0;
                for (int p = j - LookbackPeriods + 1; p <= j; p++)
                {
                    sumTr += Tr.Increment(
                        (double)ProviderCache[p].High,
                        (double)ProviderCache[p].Low,
                        (double)ProviderCache[p - 1].Close);
                }
                atr = sumTr / LookbackPeriods;
            }

            PrevAtr = atr;

            // Calculate SAR (for stop detection, not needed to store)
            double arc = atr * Multiplier;
            double sar = IsLong ? Sic - arc : Sic + arc;

            // Check for stop
            if ((IsLong && close < sar) || (!IsLong && close > sar))
            {
                // Stop detected
                if (!FirstStopFound)
                {
                    FirstStopFound = true;
                }
                Sic = close;
                IsLong = !IsLong;
            }
            else
            {
                // Update significant close
                Sic = IsLong ? Math.Max(Sic, close) : Math.Min(Sic, close);
            }
        }
    }

}


public static partial class VolatilityStop
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VolatilityStopHub"/> class.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">The multiplier for the ATR.</param>
    /// <returns>An instance of <see cref="VolatilityStopHub"/>.</returns>
    public static VolatilityStopHub ToVolatilityStopHub(
       this IQuoteProvider<IQuote> quoteProvider,
       int lookbackPeriods = 7,
       double multiplier = 3)
           => new(quoteProvider, lookbackPeriods, multiplier);

    /// <summary>
    /// Creates a VolatilityStop hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">The multiplier for the ATR.</param>
    /// <returns>An instance of <see cref="VolatilityStopHub"/>.</returns>
    public static VolatilityStopHub ToVolatilityStopHub(
        this IReadOnlyList<IQuote> quotes,
       int lookbackPeriods = 7,
       double multiplier = 3)
    {
        ArgumentNullException.ThrowIfNull(quotes);
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToVolatilityStopHub(lookbackPeriods, multiplier);
    }
}
