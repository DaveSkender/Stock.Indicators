namespace Skender.Stock.Indicators;

public static partial class Ultimate
{
    /// <summary>
    /// Converts the provided quote provider to an Ultimate Oscillator hub with the specified periods.
    /// </summary>
    /// <typeparam name="TIn">The type of the input quote.</typeparam>
    /// <param name="quoteProvider">The quote provider to convert.</param>
    /// <param name="shortPeriods">The number of short lookback periods. Default is 7.</param>
    /// <param name="middlePeriods">The number of middle lookback periods. Default is 14.</param>
    /// <param name="longPeriods">The number of long lookback periods. Default is 28.</param>
    /// <returns>An instance of <see cref="UltimateHub{TIn}"/>.</returns>
    public static UltimateHub<TIn> ToUltimateHub<TIn>(
        this IQuoteProvider<TIn> quoteProvider,
        int shortPeriods = 7,
        int middlePeriods = 14,
        int longPeriods = 28)
        where TIn : IQuote
        => new(quoteProvider, shortPeriods, middlePeriods, longPeriods);
}

/// <summary>
/// Streaming hub for calculating the Ultimate Oscillator indicator.
/// </summary>
/// <typeparam name="TIn">The type of the input quote.</typeparam>
public class UltimateHub<TIn>
    : ChainProvider<TIn, UltimateResult>, IUltimate
    where TIn : IQuote
{
    #region constructors

    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="UltimateHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="shortPeriods">The number of short lookback periods.</param>
    /// <param name="middlePeriods">The number of middle lookback periods.</param>
    /// <param name="longPeriods">The number of long lookback periods.</param>
    internal UltimateHub(
        IQuoteProvider<TIn> provider,
        int shortPeriods,
        int middlePeriods,
        int longPeriods)
        : base(provider)
    {
        Ultimate.Validate(shortPeriods, middlePeriods, longPeriods);
        ShortPeriods = shortPeriods;
        MiddlePeriods = middlePeriods;
        LongPeriods = longPeriods;
        hubName = $"UO({shortPeriods},{middlePeriods},{longPeriods})";

        Reinitialize();
    }
    #endregion

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
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (UltimateResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
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
            TIn current = ProviderCache[p];
            TIn previous = ProviderCache[p - 1];

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
