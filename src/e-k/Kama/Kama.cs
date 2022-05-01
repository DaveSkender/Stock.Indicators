namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // KAUFMAN's ADAPTIVE MOVING AVERAGE
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<KamaResult> GetKama<TQuote>(
        this IEnumerable<TQuote> quotes,
        int erPeriods = 10,
        int fastPeriods = 2,
        int slowPeriods = 30)
        where TQuote : IQuote
    {
        // convert quotes
        List<BasicData> quotesList = quotes.ToBasicClass(CandlePart.Close);

        // check parameter arguments
        ValidateKama(erPeriods, fastPeriods, slowPeriods);

        // initialize
        List<KamaResult> results = new(quotesList.Count);
        double scFast = 2d / (fastPeriods + 1);
        double scSlow = 2d / (slowPeriods + 1);

        // roll through quotes
        for (int i = 0; i < quotesList.Count; i++)
        {
            BasicData q = quotesList[i];

            KamaResult r = new()
            {
                Date = q.Date
            };

            if (i + 1 > erPeriods)
            {
                // ER period change
                double change = Math.Abs(q.Value - quotesList[i - erPeriods].Value);

                // volatility
                double sumPV = 0;
                for (int p = i - erPeriods + 1; p <= i; p++)
                {
                    sumPV += Math.Abs(quotesList[p].Value - quotesList[p - 1].Value);
                }

                if (sumPV != 0)
                {
                    // efficiency ratio
                    double er = change / sumPV;
                    r.ER = er;

                    // smoothing constant
                    double sc = (er * (scFast - scSlow)) + scSlow;  // squared later

                    // kama calculation
                    double? pk = (double?)results[i - 1].Kama;  // prior KAMA
                    r.Kama = (decimal?)(pk + (sc * sc * (q.Value - pk)));
                }

                // handle flatline case
                else
                {
                    r.ER = 0;
                    r.Kama = (decimal?)q.Value;
                }
            }

            // initial value
            else if (i + 1 == erPeriods)
            {
                r.Kama = (decimal?)q.Value;
            }

            results.Add(r);
        }

        return results;
    }

    // parameter validation
    private static void ValidateKama(
        int erPeriods,
        int fastPeriods,
        int slowPeriods)
    {
        // check parameter arguments
        if (erPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(erPeriods), erPeriods,
                "Efficiency Ratio periods must be greater than 0 for KAMA.");
        }

        if (fastPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fastPeriods), fastPeriods,
                "Fast EMA periods must be greater than 0 for KAMA.");
        }

        if (slowPeriods <= fastPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                "Slow EMA periods must be greater than Fast EMA period for KAMA.");
        }
    }
}
