namespace Skender.Stock.Indicators;

// CHANDE MOMENTUM OSCILLATOR (SERIES)

public static partial class Indicator
{
    internal static List<CmoResult> CalcCmo(
        this List<(DateTime Timestamp, double Value)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        Cmo.Validate(lookbackPeriods);

        // initialize
        int length = tpList.Count;
        List<CmoResult> results = new(length);
        List<(bool? isUp, double value)> ticks = new(length);

        // discontinue of empty
        if (length == 0)
        {
            return results;
        }

        // initialize, add first records
        double prevPrice = tpList[0].Value;

        results.Add(new CmoResult { Timestamp = tpList[0].Timestamp });
        ticks.Add((null, double.NaN));

        // roll through remaining prices
        for (int i = 1; i < length; i++)
        {
            (DateTime date, double price) = tpList[i];

            CmoResult r = new() { Timestamp = date };
            results.Add(r);

            // determine tick direction and size
            (bool? isUp, double value) tick = (null, Math.Abs(price - prevPrice));

            tick.isUp = double.IsNaN(tick.value) ? null
                : price > prevPrice ? true
                : price < prevPrice ? false
                : null;

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
                    else if (isUp == true)
                    {
                        sH += pDiff;
                    }

                    // down
                    else
                    {
                        sL += pDiff;
                    }
                }

                r.Cmo = (sH + sL != 0)
                    ? (100 * (sH - sL) / (sH + sL)).NaN2Null()
                    : null;
            }

            prevPrice = price;
        }

        return results;
    }
}
