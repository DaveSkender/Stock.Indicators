namespace Skender.Stock.Indicators;

public static partial class Alligator
{
    private static double SmoothValue<T>(
        List<T> source,
        AlligatorResult[] results,
        int i,
        int periods,
        int offset,
        Func<AlligatorResult, double?> selector)
    where T : IReusable
    {
        // skip warmup periods
        if (i < periods + offset - 1)
        {
            return double.NaN;
        }

        // re/initialize
        if (results[i - 1] is null || selector(results[i - 1]) is null)
        {
            double sum = 0;
            for (int p = i - periods - offset + 1; p <= i - offset; p++)
            {
                sum += source[p].Value;
            }
            return sum / periods;
        }

        // normal smoothing
        else
        {
            double prevValue = selector(results[i - 1]).Null2NaN();
            return ((prevValue * (periods - 1)) + source[i - offset].Value) / periods;
        }
    }

    // CONDENSE (REMOVE null results)
    public static IReadOnlyList<AlligatorResult> Condense(
        this IEnumerable<AlligatorResult> results)
    {
        List<AlligatorResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.Jaw is null && x.Teeth is null && x.Lips is null);

        return resultsList.ToSortedList();
    }

    // remove recommended periods
    public static IReadOnlyList<AlligatorResult> RemoveWarmupPeriods(
        this IEnumerable<AlligatorResult> results)
    {
        int removePeriods = results
          .ToList()
          .FindIndex(x => x.Jaw != null) + 251;

        return results.Remove(removePeriods);
    }

    // parameter validation
    internal static void Validate(
        int jawPeriods,
        int jawOffset,
        int teethPeriods,
        int teethOffset,
        int lipsPeriods,
        int lipsOffset)
    {
        // check parameter arguments
        if (jawPeriods <= teethPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(jawPeriods), jawPeriods,
                "Jaw lookback periods must be greater than Teeth lookback periods for Alligator.");
        }

        if (teethPeriods <= lipsPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(teethPeriods), teethPeriods,
                "Teeth lookback periods must be greater than Lips lookback periods for Alligator.");
        }

        if (lipsPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lipsPeriods), lipsPeriods,
                "Lips lookback periods must be greater than 0 for Alligator.");
        }

        if (jawOffset <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(jawOffset), jawOffset,
                "Jaw offset periods must be greater than 0 for Alligator.");
        }

        if (teethOffset <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(teethOffset), teethOffset,
                "Jaw offset periods must be greater than 0 for Alligator.");
        }

        if (lipsOffset <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lipsOffset), lipsOffset,
                "Jaw offset periods must be greater than 0 for Alligator.");
        }

        if (jawPeriods + jawOffset <= teethPeriods + teethOffset)
        {
            throw new ArgumentOutOfRangeException(nameof(jawPeriods), jawPeriods,
                "Jaw lookback + offset are too small for Alligator.");
        }

        if (teethPeriods + teethOffset <= lipsPeriods + lipsOffset)
        {
            throw new ArgumentOutOfRangeException(nameof(teethPeriods), teethPeriods,
                "Teeth lookback + offset are too small for Alligator.");
        }
    }
}
