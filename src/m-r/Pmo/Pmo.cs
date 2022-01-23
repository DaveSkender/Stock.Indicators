namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // PRICE MOMENTUM OSCILLATOR (PMO)
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<PmoResult> GetPmo<TQuote>(
        this IEnumerable<TQuote> quotes,
        int timePeriods = 35,
        int smoothPeriods = 20,
        int signalPeriods = 10)
        where TQuote : IQuote
    {
        // check parameter arguments
        ValidatePmo(timePeriods, smoothPeriods, signalPeriods);

        // initialize
        List<PmoResult> results = CalcPmoRocEma(quotes, timePeriods);
        double smoothingConstant = 2d / smoothPeriods;
        double? lastPmo = null;

        // calculate PMO
        int startIndex = timePeriods + smoothPeriods;

        for (int i = startIndex - 1; i < results.Count; i++)
        {
            PmoResult pr = results[i];
            int index = i + 1;

            if (index > startIndex)
            {
                pr.Pmo = ((pr.RocEma - lastPmo) * smoothingConstant) + lastPmo;
            }
            else if (index == startIndex)
            {
                double? sumRocEma = 0;
                for (int p = index - smoothPeriods; p < index; p++)
                {
                    PmoResult d = results[p];
                    sumRocEma += d.RocEma;
                }

                pr.Pmo = sumRocEma / smoothPeriods;
            }

            lastPmo = pr.Pmo;
        }

        // add Signal
        CalcPmoSignal(results, timePeriods, smoothPeriods, signalPeriods);

        return results;
    }

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<PmoResult> RemoveWarmupPeriods(
        this IEnumerable<PmoResult> results)
    {
        int ts = results
            .ToList()
            .FindIndex(x => x.Pmo != null) + 1;

        return results.Remove(ts + 250);
    }

    // internals
    private static List<PmoResult> CalcPmoRocEma<TQuote>(
        IEnumerable<TQuote> quotes,
        int timePeriods)
        where TQuote : IQuote
    {
        // initialize
        double smoothingMultiplier = 2d / timePeriods;
        double? lastRocEma = null;
        List<RocResult> roc = GetRoc(quotes, 1).ToList();
        List<PmoResult> results = new();

        int startIndex = timePeriods + 1;

        for (int i = 0; i < roc.Count; i++)
        {
            RocResult r = roc[i];
            int index = i + 1;

            PmoResult result = new()
            {
                Date = r.Date
            };

            if (index > startIndex)
            {
                result.RocEma = (r.Roc * smoothingMultiplier) + (lastRocEma * (1 - smoothingMultiplier));
            }
            else if (index == startIndex)
            {
                double? sumRoc = 0;
                for (int p = index - timePeriods; p < index; p++)
                {
                    RocResult d = roc[p];
                    sumRoc += d.Roc;
                }

                result.RocEma = sumRoc / timePeriods;
            }

            lastRocEma = result.RocEma;
            result.RocEma *= 10;
            results.Add(result);
        }

        return results;
    }

    private static void CalcPmoSignal(
        List<PmoResult> results,
        int timePeriods,
        int smoothPeriods,
        int signalPeriods)
    {
        double signalConstant = 2d / (signalPeriods + 1);
        double? lastSignal = null;

        int startIndex = timePeriods + smoothPeriods + signalPeriods - 1;

        for (int i = startIndex - 1; i < results.Count; i++)
        {
            PmoResult pr = results[i];
            int index = i + 1;

            if (index > startIndex)
            {
                pr.Signal = ((pr.Pmo - lastSignal) * signalConstant) + lastSignal;
            }
            else if (index == startIndex)
            {
                double? sumPmo = 0;
                for (int p = index - signalPeriods; p < index; p++)
                {
                    PmoResult d = results[p];
                    sumPmo += d.Pmo;
                }

                pr.Signal = sumPmo / signalPeriods;
            }

            lastSignal = pr.Signal;
        }
    }

    // parameter validation
    private static void ValidatePmo(
        int timePeriods,
        int smoothPeriods,
        int signalPeriods)
    {
        // check parameter arguments
        if (timePeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(timePeriods), timePeriods,
                "Time periods must be greater than 1 for PMO.");
        }

        if (smoothPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smoothPeriods), smoothPeriods,
                "Smoothing periods must be greater than 0 for PMO.");
        }

        if (signalPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                "Signal periods must be greater than 0 for PMO.");
        }
    }
}
