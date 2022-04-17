namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // AWESOME OSCILLATOR
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<AwesomeResult> GetAwesome<TQuote>(
        this IEnumerable<TQuote> quotes,
        int fastPeriods = 5,
        int slowPeriods = 34)
        where TQuote : IQuote
    {
        // convert quotes
        List<Price> bdList = quotes.ToPrice(CandlePart.HL2);

        // check parameter arguments
        ValidateAwesome(fastPeriods, slowPeriods);

        // initialize
        int length = bdList.Count;
        List<AwesomeResult> results = new();
        double[] pr = new double[length]; // median price

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            Price q = bdList[i];
            pr[i] = q.Value;
            int index = i + 1;

            AwesomeResult r = new()
            {
                Date = q.Date
            };

            if (index >= slowPeriods)
            {
                double sumSlow = 0;
                double sumFast = 0;

                for (int p = index - slowPeriods; p < index; p++)
                {
                    sumSlow += pr[p];

                    if (p >= index - fastPeriods)
                    {
                        sumFast += pr[p];
                    }
                }

                r.Oscillator = (sumFast / fastPeriods) - (sumSlow / slowPeriods);
                r.Normalized = (pr[i] != 0) ? 100 * r.Oscillator / pr[i] : null;
            }

            results.Add(r);
        }

        return results;
    }

    // parameter validation
    private static void ValidateAwesome(
        int fastPeriods,
        int slowPeriods)
    {
        // check parameter arguments
        if (fastPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                "Fast periods must be greater than 0 for Awesome Oscillator.");
        }

        if (slowPeriods <= fastPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                "Slow periods must be larger than Fast Periods for Awesome Oscillator.");
        }
    }
}
