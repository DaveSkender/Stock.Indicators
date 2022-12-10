namespace Skender.Stock.Indicators;

// CHANDE MOMENTUM OSCILLATOR (SERIES)
public static partial class Indicator
{
    internal static List<CmoResult> CalcCmo(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateCmo(lookbackPeriods);

        // initialize
        int length = tpList.Count;
        List<CmoResult> results = new(length);
        List<(bool? isUp, double value)> ticks = new(length);
        double prevValue = double.NaN;

        // add initial record
        if (length > 0)
        {
            results.Add(new CmoResult(tpList[0].Item1));
            ticks.Add((null, double.NaN));

            prevValue = tpList[0].Item2;
        }

        // roll through quotes
        for (int i = 1; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];

            CmoResult r = new(date);
            results.Add(r);
            ticks.Add((
                  value > prevValue ? true
                : value < prevValue ? false
                : null, Math.Abs(value - prevValue)));

            if (i >= lookbackPeriods)
            {
                double sH = 0;
                double sL = 0;

                for (int p = i - lookbackPeriods + 1; p <= i; p++)
                {
                    (bool? isUp, double pDiff) = ticks[p];

                    if (isUp is null)
                    {
                        continue;
                    }
                    else if (isUp == true)
                    {
                        sH += pDiff;
                    }
                    else
                    {
                        sL += pDiff;
                    }
                }

                r.Cmo = (sH + sL != 0)
                    ? (100 * (sH - sL) / (sH + sL)).NaN2Null()
                    : null;
            }

            prevValue = value;
        }

        return results;
    }

    // parameter validation
    private static void ValidateCmo(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for CMO.");
        }
    }
}
