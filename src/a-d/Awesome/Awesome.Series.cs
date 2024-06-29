namespace Skender.Stock.Indicators;

// AWESOME OSCILLATOR (SERIES)

public static partial class Indicator
{
    internal static List<AwesomeResult> CalcAwesome<T>(
        this List<T> source,
        int fastPeriods,
        int slowPeriods)
        where T : IReusable
    {
        // check parameter arguments
        Awesome.Validate(fastPeriods, slowPeriods);

        // use standard HL2 if quote source (override Close)
        List<IReusable> feed
            = typeof(IQuote).IsAssignableFrom(typeof(T))

            ? source
             .Cast<IQuote>()
             .Use(CandlePart.HL2)
             .Cast<IReusable>()
             .ToSortedList()

            : source
             .Cast<IReusable>()
             .ToSortedList();

        // initialize
        int length = source.Count;
        List<AwesomeResult> results = new(length);
        double[] pr = new double[length];

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            IReusable s = feed[i];
            pr[i] = s.Value;

            double? oscillator = null;
            double? normalized = null;

            if (i >= slowPeriods - 1)
            {
                double sumSlow = 0;
                double sumFast = 0;

                for (int p = i + 1 - slowPeriods; p <= i; p++)
                {
                    sumSlow += pr[p];

                    if (p >= i + 1 - fastPeriods)
                    {
                        sumFast += pr[p];
                    }
                }

                oscillator = ((sumFast / fastPeriods) - (sumSlow / slowPeriods)).NaN2Null();
                normalized = (pr[i] != 0) ? 100 * oscillator / pr[i] : null;
            }

            AwesomeResult r = new(
                Timestamp: s.Timestamp,
                Oscillator: oscillator,
                Normalized: normalized);

            results.Add(r);
        }

        return results;
    }
}
