namespace Skender.Stock.Indicators;

// PRICE MOMENTUM OSCILLATOR (SERIES)
public static partial class Indicator
{
    internal static List<PmoResult> CalcPmo(
        this List<(DateTime, double)> tpList,
        int timePeriods,
        int smoothPeriods,
        int signalPeriods)
    {
        // check parameter arguments
        ValidatePmo(timePeriods, smoothPeriods, signalPeriods);

        // initialize
        List<PmoResult> results = tpList.CalcPmoRocEma(timePeriods);
        double smoothingConstant = 2d / smoothPeriods;
        double? lastPmo = null;

        // calculate PMO
        int startIndex = timePeriods + smoothPeriods;

        for (int i = startIndex - 1; i < results.Count; i++)
        {
            PmoResult pr = results[i];

            if (i + 1 > startIndex)
            {
                pr.Pmo = ((pr.RocEma - lastPmo) * smoothingConstant) + lastPmo;
            }
            else if (i + 1 == startIndex)
            {
                double? sumRocEma = 0;
                for (int p = i + 1 - smoothPeriods; p <= i; p++)
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

    // internals
    private static List<PmoResult> CalcPmoRocEma(
        this List<(DateTime, double)> tpList,
        int timePeriods)
    {
        // initialize
        double smoothingMultiplier = 2d / timePeriods;
        double? lastRocEma = null;
        List<RocResult> roc = tpList.CalcRoc(1, null).ToList();
        List<PmoResult> results = new();

        int startIndex = timePeriods + 1;

        for (int i = 0; i < roc.Count; i++)
        {
            RocResult rocResult = roc[i];

            PmoResult r = new(rocResult.Date);
            results.Add(r);

            if (i + 1 > startIndex)
            {
                r.RocEma = (rocResult.Roc * smoothingMultiplier) + (lastRocEma * (1 - smoothingMultiplier));
            }
            else if (i + 1 == startIndex)
            {
                double? sumRoc = 0;
                for (int p = i + 1 - timePeriods; p <= i; p++)
                {
                    RocResult d = roc[p];
                    sumRoc += d.Roc;
                }

                r.RocEma = sumRoc / timePeriods;
            }

            lastRocEma = r.RocEma;
            r.RocEma *= 10;
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

            if (i + 1 > startIndex)
            {
                pr.Signal = ((pr.Pmo - lastSignal) * signalConstant) + lastSignal;
            }
            else if (i + 1 == startIndex)
            {
                double? sumPmo = 0;
                for (int p = i + 1 - signalPeriods; p <= i; p++)
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
