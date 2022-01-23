namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // ULTIMATE OSCILLATOR
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<UltimateResult> GetUltimate<TQuote>(
        this IEnumerable<TQuote> quotes,
        int shortPeriods = 7,
        int middlePeriods = 14,
        int longPeriods = 28)
        where TQuote : IQuote
    {
        // convert quotes
        List<QuoteD> quotesList = quotes.ConvertToList();

        // check parameter arguments
        ValidateUltimate(shortPeriods, middlePeriods, longPeriods);

        // initialize
        int size = quotesList.Count;
        List<UltimateResult> results = new(size);
        double[] bp = new double[size]; // buying pressure
        double[] tr = new double[size]; // true range

        double priorClose = 0;

        // roll through quotes
        for (int i = 0; i < quotesList.Count; i++)
        {
            QuoteD q = quotesList[i];
            int index = i + 1;

            UltimateResult r = new()
            {
                Date = q.Date
            };
            results.Add(r);

            if (i > 0)
            {
                bp[i] = q.Close - Math.Min(q.Low, priorClose);
                tr[i] = Math.Max(q.High, priorClose) - Math.Min(q.Low, priorClose);
            }

            if (index >= longPeriods + 1)
            {
                double sumBP1 = 0;
                double sumBP2 = 0;
                double sumBP3 = 0;

                double sumTR1 = 0;
                double sumTR2 = 0;
                double sumTR3 = 0;

                for (int p = index - longPeriods; p < index; p++)
                {
                    int pIndex = p + 1;

                    // short aggregate
                    if (pIndex > index - shortPeriods)
                    {
                        sumBP1 += bp[p];
                        sumTR1 += tr[p];
                    }

                    // middle aggregate
                    if (pIndex > index - middlePeriods)
                    {
                        sumBP2 += bp[p];
                        sumTR2 += tr[p];
                    }

                    // long aggregate
                    sumBP3 += bp[p];
                    sumTR3 += tr[p];
                }

                double? avg1 = (sumTR1 == 0) ? null : sumBP1 / sumTR1;
                double? avg2 = (sumTR2 == 0) ? null : sumBP2 / sumTR2;
                double? avg3 = (sumTR3 == 0) ? null : sumBP3 / sumTR3;

                r.Ultimate = (decimal?)(100d * ((4d * avg1) + (2d * avg2) + avg3) / 7d);
            }

            priorClose = q.Close;
        }

        return results;
    }

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<UltimateResult> RemoveWarmupPeriods(
        this IEnumerable<UltimateResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Ultimate != null);

        return results.Remove(removePeriods);
    }

    // parameter validation
    private static void ValidateUltimate(
        int shortPeriods,
        int middleAverage,
        int longPeriods)
    {
        // check parameter arguments
        if (shortPeriods <= 0 || middleAverage <= 0 || longPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(longPeriods), longPeriods,
                "Average periods must be greater than 0 for Ultimate Oscillator.");
        }

        if (shortPeriods >= middleAverage || middleAverage >= longPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(middleAverage), middleAverage,
                "Average periods must be increasingly larger than each other for Ultimate Oscillator.");
        }
    }
}
