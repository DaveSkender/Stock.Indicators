namespace Skender.Stock.Indicators;

/// <inheritdoc />
public class UltimateHub
    : ChainProvider<IReusable, UltimateResult>, IUltimate
{


    /// <summary>
    /// Initializes a new instance of the <see cref="UltimateHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="shortPeriods">The number of short lookback periods.</param>
    /// <param name="middlePeriods">The number of middle lookback periods.</param>
    /// <param name="longPeriods">The number of long lookback periods.</param>
    internal UltimateHub(
        IQuoteProvider<IQuote> provider,
        int shortPeriods,
        int middlePeriods,
        int longPeriods)
        : base(provider)
    {
        Ultimate.Validate(shortPeriods, middlePeriods, longPeriods);
        ShortPeriods = shortPeriods;
        MiddlePeriods = middlePeriods;
        LongPeriods = longPeriods;
        Name = $"UO({shortPeriods},{middlePeriods},{longPeriods})";

        Reinitialize();
    }

    /// <summary>
    /// Gets the number of short lookback periods.
    /// </summary>
    public int ShortPeriods { get; init; }

    /// <summary>
    /// Gets the number of middle lookback periods.
    /// </summary>
    public int MiddlePeriods { get; init; }

    /// <summary>
    /// Gets the number of long lookback periods.
    /// </summary>
    public int LongPeriods { get; init; }

    // METHODS
    /// <inheritdoc/>
    protected override (UltimateResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // skip first period (need previous close)
        if (i == 0)
        {
            return (new UltimateResult(item.Timestamp, null), i);
        }

        // not enough data for calculation
        if (i < LongPeriods)
        {
            return (new UltimateResult(item.Timestamp, null), i);
        }

        // calculate Ultimate Oscillator
        double sumBp1 = 0;  // short period
        double sumBp2 = 0;  // middle period
        double sumBp3 = 0;  // long period

        double sumTr1 = 0;  // short period
        double sumTr2 = 0;  // middle period
        double sumTr3 = 0;  // long period

        // Calculate sums for all three periods
        for (int p = i - LongPeriods + 1; p <= i; p++)
        {
            IQuote current = (IQuote)ProviderCache[p];
            IQuote previous = (IQuote)ProviderCache[p - 1];

            double high = (double)current.High;
            double low = (double)current.Low;
            double close = (double)current.Close;
            double prevClose = (double)previous.Close;

            // Calculate buying pressure and true range
            double bp = close - Math.Min(low, prevClose);
            double tr = Math.Max(high, prevClose) - Math.Min(low, prevClose);

            // Long period includes all values
            sumBp3 += bp;
            sumTr3 += tr;

            // Middle period includes more recent values
            if (p > i - MiddlePeriods)
            {
                sumBp2 += bp;
                sumTr2 += tr;
            }

            // Short period includes most recent values
            if (p > i - ShortPeriods)
            {
                sumBp1 += bp;
                sumTr1 += tr;
            }
        }

        // Calculate averages (avoid division by zero)
        double avg1 = sumTr1 == 0 ? double.NaN : sumBp1 / sumTr1;
        double avg2 = sumTr2 == 0 ? double.NaN : sumBp2 / sumTr2;
        double avg3 = sumTr3 == 0 ? double.NaN : sumBp3 / sumTr3;

        // Calculate Ultimate Oscillator with weighted average
        double ultimate = (100d * ((4d * avg1) + (2d * avg2) + avg3) / 7d).NaN2Null() ?? double.NaN;

        UltimateResult r = new(
            item.Timestamp,
            ultimate.NaN2Null());

        return (r, i);
    }
}

public static partial class Ultimate
{
    /// <summary>
    /// Converts the provided quote provider to an Ultimate Oscillator hub with the specified periods.
    /// </summary>
    /// <param name="quoteProvider">The quote provider to convert.</param>
    /// <param name="shortPeriods">The number of short lookback periods.</param>
    /// <param name="middlePeriods">The number of middle lookback periods.</param>
    /// <param name="longPeriods">The number of long lookback periods.</param>
    /// <returns>An instance of <see cref="UltimateHub"/>.</returns>
    public static UltimateHub ToUltimateHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int shortPeriods = 7,
        int middlePeriods = 14,
        int longPeriods = 28)
             => new(quoteProvider, shortPeriods, middlePeriods, longPeriods);

    /// <summary>
    /// Creates a Ultimate hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="shortPeriods">Parameter for the calculation.</param>
    /// <param name="middlePeriods">Parameter for the calculation.</param>
    /// <param name="longPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="UltimateHub"/>.</returns>
    public static UltimateHub ToUltimateHub(
        this IReadOnlyList<IQuote> quotes,
        int shortPeriods = 7,
        int middlePeriods = 14,
        int longPeriods = 28)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToUltimateHub(shortPeriods, middlePeriods, longPeriods);
    }

}
