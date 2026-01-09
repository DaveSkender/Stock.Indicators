namespace Skender.Stock.Indicators;

/// <inheritdoc />
public class UltimateHub
    : ChainHub<IReusable, UltimateResult>, IUltimate
{
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

    /// <inheritdoc/>
    public int ShortPeriods { get; init; }

    /// <inheritdoc/>
    public int MiddlePeriods { get; init; }

    /// <inheritdoc/>
    public int LongPeriods { get; init; }

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
}
