namespace Skender.Stock.Indicators;

// CHANDE MOMENTUM OSCILLATOR (SERIES)

public static partial class Indicator
{
    private static List<CmoResult> CalcCmo<T>(
        this List<T> source,
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        Cmo.Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<CmoResult> results = new(length);
        List<(bool? isUp, double value)> ticks = new(length);

        // discontinue of empty
        if (length == 0)
        {
            return results;
        }

        // initialize, add first records
        double prevValue = source[0].Value;

        results.Add(new(source[0].Timestamp));
        ticks.Add((null, double.NaN));

        // roll through remaining prices
        for (int i = 1; i < length; i++)
        {
            T s = source[i];
            double? cmo = null;

            // determine tick direction and size
            (bool? isUp, double value) tick = (null, Math.Abs(s.Value - prevValue));

            tick.isUp = double.IsNaN(tick.value) ? null
                : (s.Value > prevValue ? true
                    : (s.Value < prevValue ? false
                        : null));

            ticks.Add(tick);

            // calculate CMO
            if (i >= lookbackPeriods)
            {
                double sH = 0;
                double sL = 0;

                for (int p = i - lookbackPeriods + 1; p <= i; p++)
                {
                    (bool? isUp, double pDiff) = ticks[p];

                    if (double.IsNaN(pDiff))
                    {
                        sH = double.NaN;
                        sL = double.NaN;
                        break;
                    }

                    // up

                    if (isUp == true)
                    {
                        sH += pDiff;
                    }

                    // down
                    else
                    {
                        sL += pDiff;
                    }
                }

                cmo = sH + sL != 0
                    ? (100 * (sH - sL) / (sH + sL)).NaN2Null()
                    : null;
            }

            results.Add(new(
                Timestamp: s.Timestamp,
                Cmo: cmo));

            prevValue = s.Value;
        }

        return results;
    }
}
