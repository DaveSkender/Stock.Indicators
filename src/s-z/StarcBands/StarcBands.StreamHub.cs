namespace Skender.Stock.Indicators;

// STARC BANDS (STREAM HUB)

/// <summary>
/// Represents a stream hub for calculating STARC Bands.
/// </summary>
public class StarcBandsHub
    : StreamHub<IQuote, StarcBandsResult>, IStarcBands
{
    #region constructors

    private readonly string hubName;
    private readonly int _lookbackPeriods;
    private double _prevAtr = double.NaN;

    /// <summary>
    /// Initializes a new instance of the <see cref="StarcBandsHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="smaPeriods">The number of periods for the Simple Moving Average (SMA).</param>
    /// <param name="multiplier">The multiplier for the Average True Range (ATR).</param>
    /// <param name="atrPeriods">The number of periods for the ATR calculation.</param>
    internal StarcBandsHub(
        IQuoteProvider<IQuote> provider,
        int smaPeriods,
        double multiplier,
        int atrPeriods) : base(provider)
    {
        StarcBands.Validate(smaPeriods, multiplier, atrPeriods);

        SmaPeriods = smaPeriods;
        Multiplier = multiplier;
        AtrPeriods = atrPeriods;
        _lookbackPeriods = Math.Max(smaPeriods, atrPeriods);
        hubName = $"STARC({smaPeriods},{multiplier},{atrPeriods})";

        Reinitialize();
    }

    #endregion constructors

    #region properties

    /// <inheritdoc/>
    public int SmaPeriods { get; init; }

    /// <inheritdoc/>
    public double Multiplier { get; init; }

    /// <inheritdoc/>
    public int AtrPeriods { get; init; }

    #endregion properties

    #region methods

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Reset ATR state
        _prevAtr = double.NaN;

        // Find the index for the timestamp
        int index = ProviderCache.IndexGte(timestamp);
        if (index <= 0)
        {
            return;
        }

        int targetIndex = index - 1;

        // Rebuild ATR state up to the target index
        if (targetIndex >= AtrPeriods)
        {
            // Initialize ATR
            double sumTr = 0;
            for (int p = 1; p <= AtrPeriods; p++)
            {
                sumTr += Tr.Increment(
                    (double)ProviderCache[p].High,
                    (double)ProviderCache[p].Low,
                    (double)ProviderCache[p - 1].Close);
            }

            double prevAtr = sumTr / AtrPeriods;

            // Incrementally update ATR from AtrPeriods+1 to targetIndex
            for (int p = AtrPeriods + 1; p <= targetIndex; p++)
            {
                double tr = Tr.Increment(
                    (double)ProviderCache[p].High,
                    (double)ProviderCache[p].Low,
                    (double)ProviderCache[p - 1].Close);

                prevAtr = ((prevAtr * (AtrPeriods - 1)) + tr) / AtrPeriods;
            }

            _prevAtr = prevAtr;
        }
    }

    /// <summary>
    /// Calculates the simple moving average of Close prices.
    /// </summary>
    /// <param name="endIndex">Ending index for calculation</param>
    /// <param name="periods">Number of periods</param>
    private double CalculateSmaOfClose(int endIndex, int periods)
    {
        if (endIndex < periods - 1 || endIndex + 1 > ProviderCache.Count)
        {
            return double.NaN;
        }

        double sum = 0;
        for (int i = endIndex - periods + 1; i <= endIndex; i++)
        {
            sum += (double)ProviderCache[i].Close;
        }

        return sum / periods;
    }

    /// <inheritdoc/>
    protected override (StarcBandsResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Calculate SMA of Close
        double sma;
        if (i >= SmaPeriods - 1 && Cache[i - 1].Centerline is not null)
        {
            // Calculate SMA incrementally
            // We need to recalculate from cache since SMA doesn't have an efficient incremental formula
            sma = CalculateSmaOfClose(i, SmaPeriods);
        }
        else if (i >= SmaPeriods - 1)
        {
            // Initialize as SMA of Close prices
            sma = CalculateSmaOfClose(i, SmaPeriods);
        }
        else
        {
            // warmup periods are never calculable
            sma = double.NaN;
        }

        // Calculate ATR
        double atr;

        if (i == 0)
        {
            atr = double.NaN;
        }
        else if (!double.IsNaN(_prevAtr) && i >= AtrPeriods)
        {
            // Calculate ATR normally using previous ATR
            AtrResult atrResult = Atr.Increment(AtrPeriods, item, (double)ProviderCache[i - 1].Close, _prevAtr);
            atr = atrResult.Atr ?? double.NaN;
            _prevAtr = atr;
        }
        else if (i >= AtrPeriods)
        {
            // Initialize ATR using same method as Series:
            // Sum TR from index 1 to AtrPeriods, then incrementally update to current index
            double sumTr = 0;

            // Initial sum from index 1 to AtrPeriods (matching Series behavior)
            for (int p = 1; p <= AtrPeriods; p++)
            {
                sumTr += Tr.Increment(
                    (double)ProviderCache[p].High,
                    (double)ProviderCache[p].Low,
                    (double)ProviderCache[p - 1].Close);
            }

            double prevAtr = sumTr / AtrPeriods;

            // Incrementally update ATR from AtrPeriods+1 to i
            for (int p = AtrPeriods + 1; p <= i; p++)
            {
                double tr = Tr.Increment(
                    (double)ProviderCache[p].High,
                    (double)ProviderCache[p].Low,
                    (double)ProviderCache[p - 1].Close);

                prevAtr = ((prevAtr * (AtrPeriods - 1)) + tr) / AtrPeriods;
            }

            atr = prevAtr;
            _prevAtr = atr;
        }
        else
        {
            atr = double.NaN;
        }

        // Calculate result based on what's available
        double? centerline = sma.NaN2Null();
        double? upperBand = null;
        double? lowerBand = null;

        // Calculate bands only when both SMA and ATR are available
        if (centerline.HasValue && atr.NaN2Null().HasValue)
        {
            double? atrSpan = atr.NaN2Null() * Multiplier;
            upperBand = centerline + atrSpan;
            lowerBand = centerline - atrSpan;
        }

        StarcBandsResult r = new(
            Timestamp: item.Timestamp,
            UpperBand: upperBand,
            Centerline: centerline,
            LowerBand: lowerBand);

        return (r, i);
    }

    #endregion methods
}


public static partial class StarcBands
{
    /// <summary>
    /// Creates a STARC Bands streaming hub from a quote provider.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="smaPeriods">The number of periods for the Simple Moving Average (SMA).</param>
    /// <param name="multiplier">The multiplier for the Average True Range (ATR).</param>
    /// <param name="atrPeriods">The number of periods for the ATR calculation.</param>
    /// <returns>An instance of <see cref="StarcBandsHub"/>.</returns>
    public static StarcBandsHub ToStarcBandsHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int smaPeriods = 5,
        double multiplier = 2,
        int atrPeriods = 10)
        => new(quoteProvider, smaPeriods, multiplier, atrPeriods);

    /// <summary>
    /// Creates a STARC Bands hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="smaPeriods">The number of periods for the Simple Moving Average (SMA).</param>
    /// <param name="multiplier">The multiplier for the Average True Range (ATR).</param>
    /// <param name="atrPeriods">The number of periods for the ATR calculation.</param>
    /// <returns>An instance of <see cref="StarcBandsHub"/>.</returns>
    public static StarcBandsHub ToStarcBandsHub(
        this IReadOnlyList<IQuote> quotes,
        int smaPeriods = 5,
        double multiplier = 2,
        int atrPeriods = 10)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToStarcBandsHub(smaPeriods, multiplier, atrPeriods);
    }
}
