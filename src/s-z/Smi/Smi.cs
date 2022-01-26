namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // STOCHASTIC MOMENTUM INDEX
    /// <include file='./info.xml' path='indicator/type[@name="Main"]/*' />
    ///
    public static IEnumerable<SmiResult> GetSmi<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        int firstSmoothPeriods,
        int secondSmoothPeriods,
        int signalPeriods = 3)
        where TQuote : IQuote
    {
        // convert quotes
        List<QuoteD> quotesList = quotes.ConvertToList();

        // check parameter arguments
        ValidateSmi(
            lookbackPeriods,
            firstSmoothPeriods,
            secondSmoothPeriods,
            signalPeriods);

        // initialize
        int length = quotesList.Count;
        List<SmiResult> results = new(length);

        double k1 = 2d / (firstSmoothPeriods + 1);
        double k2 = 2d / (secondSmoothPeriods + 1);
        double kS = 2d / (signalPeriods + 1);

        double lastSmEma1 = 0;
        double lastSmEma2 = 0;
        double lastHlEma1 = 0;
        double lastHlEma2 = 0;
        double lastSignal = 0;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            QuoteD q = quotesList[i];
            int index = i + 1;

            SmiResult r = new()
            {
                Date = q.Date
            };

            if (index >= lookbackPeriods)
            {
                double hH = double.MinValue;
                double lL = double.MaxValue;

                for (int p = index - lookbackPeriods; p < index; p++)
                {
                    QuoteD x = quotesList[p];

                    if (x.High > hH)
                    {
                        hH = x.High;
                    }

                    if (x.Low < lL)
                    {
                        lL = x.Low;
                    }
                }

                double sm = q.Close - (0.5d * (hH + lL));
                double hl = hH - lL;

                // initialize last EMA values
                if (index == lookbackPeriods)
                {
                    lastSmEma1 = sm;
                    lastSmEma2 = lastSmEma1;
                    lastHlEma1 = hl;
                    lastHlEma2 = lastHlEma1;
                }

                // first smoothing
                double smEma1 = lastSmEma1 + (k1 * (sm - lastSmEma1));
                double hlEma1 = lastHlEma1 + (k1 * (hl - lastHlEma1));

                // second smoothing
                double smEma2 = lastSmEma2 + (k2 * (smEma1 - lastSmEma2));
                double hlEma2 = lastHlEma2 + (k2 * (hlEma1 - lastHlEma2));

                // stochastic momentum index
                double smi = 100 * (smEma2 / (0.5 * hlEma2));
                r.Smi = (decimal)smi;

                // initialize signal line
                if (index == lookbackPeriods)
                {
                    lastSignal = smi;
                }

                // signal line
                double signal = lastSignal + (kS * (smi - lastSignal));
                r.Signal = (decimal)signal;

                // carryover values
                lastSmEma1 = smEma1;
                lastSmEma2 = smEma2;
                lastHlEma1 = hlEma1;
                lastHlEma2 = hlEma2;
                lastSignal = signal;
            }

            results.Add(r);
        }

        return results;
    }

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<SmiResult> RemoveWarmupPeriods(
        this IEnumerable<SmiResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Smi != null);

        return results.Remove(removePeriods + 2 + 100);
    }

    // parameter validation
    private static void ValidateSmi(
        int lookbackPeriods,
        int firstSmoothPeriods,
        int secondSmoothPeriods,
        int signalPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for SMI.");
        }

        if (firstSmoothPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(firstSmoothPeriods), firstSmoothPeriods,
                "Smoothing periods must be greater than 0 for SMI.");
        }

        if (secondSmoothPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(secondSmoothPeriods), secondSmoothPeriods,
                "Smoothing periods must be greater than 0 for SMI.");
        }

        if (signalPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                "Signal periods must be greater than 0 for SMI.");
        }
    }
}
