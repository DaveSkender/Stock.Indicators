namespace Skender.Stock.Indicators;

// AVERAGE DIRECTIONAL INDEX (SERIES)
public static partial class Indicator
{
    internal static List<AdxResult> CalcAdx(
        this List<QuoteD> qdList,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateAdx(lookbackPeriods);

        // initialize
        int length = qdList.Count;
        List<AdxResult> results = new(length);

        double prevHigh = 0;
        double prevLow = 0;
        double prevClose = 0;
        double prevTrs = double.NaN; // smoothed
        double prevPdm = double.NaN;
        double prevMdm = double.NaN;
        double prevAdx = double.NaN;

        double sumTr = 0;
        double sumPdm = 0;
        double sumMdm = 0;
        double sumDx = 0;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            QuoteD q = qdList[i];

            // skip first period
            if (i == 0)
            {
                prevHigh = q.High;
                prevLow = q.Low;
                prevClose = q.Close;

                results.Add(new(date: q.Date));
                continue;
            }

            double hmpc = Math.Abs(q.High - prevClose);
            double lmpc = Math.Abs(q.Low - prevClose);
            double hmph = q.High - prevHigh;
            double plml = prevLow - q.Low;

            double tr = Math.Max(q.High - q.Low, Math.Max(hmpc, lmpc));

            double pdm1 = hmph > plml ? Math.Max(hmph, 0) : 0;
            double mdm1 = plml > hmph ? Math.Max(plml, 0) : 0;

            prevHigh = q.High;
            prevLow = q.Low;
            prevClose = q.Close;

            // initialization period
            if (i <= lookbackPeriods)
            {
                sumTr += tr;
                sumPdm += pdm1;
                sumMdm += mdm1;
            }

            // skip DM initialization period
            if (i < lookbackPeriods)
            {
                results.Add(new(date: q.Date));
                continue;
            }

            // smoothed true range and directional movement
            double trs;
            double pdm;
            double mdm;

            if (double.IsNaN(prevTrs))
            {
                // re/initialize smoothed values
                trs = sumTr;
                pdm = sumPdm;
                mdm = sumMdm;
            }
            else
            {
                trs = prevTrs - (prevTrs / lookbackPeriods) + tr;
                pdm = prevPdm - (prevPdm / lookbackPeriods) + pdm1;
                mdm = prevMdm - (prevMdm / lookbackPeriods) + mdm1;
            }

            prevTrs = trs;
            prevPdm = pdm;
            prevMdm = mdm;

            if (trs == 0)
            {
                results.Add(new(date: q.Date));
                continue;
            }

            // directional increments
            double pdi = 100 * pdm / trs;
            double mdi = 100 * mdm / trs;

            // calculate ADX
            double dx = pdi == mdi
                ? 0
                : pdi + mdi != 0
                ? 100 * Math.Abs(pdi - mdi) / (pdi + mdi)
                : double.NaN;

            double adx = double.NaN;
            double adxr = double.NaN;

            // ADX initialization period - accumulate DX values
            if (i < (2 * lookbackPeriods))
            {
                sumDx += dx;

                // calculate initial ADX after accumulating lookbackPeriods DX values
                if (double.IsNaN(prevAdx) && i == (2 * lookbackPeriods) - 1)
                {
                    adx = sumDx / lookbackPeriods;
                    prevAdx = adx;
                }
            }
            // ongoing ADX smoothing
            else
            {
                adx = ((prevAdx * (lookbackPeriods - 1)) + dx) / lookbackPeriods;

                // Calculate ADXR only if we have a valid prior ADX value
                int priorAdxIndex = i - lookbackPeriods;
                if (priorAdxIndex >= (2 * lookbackPeriods) - 1)
                {
                    double priorAdx = results[priorAdxIndex].Adx.Null2NaN();

                    // Only compute ADXR if prior ADX actually exists
                    if (!double.IsNaN(priorAdx))
                    {
                        adxr = (adx + priorAdx) / 2;
                    }
                }

                prevAdx = adx;
            }

            results.Add(new(date: q.Date) {
                Pdi = pdi,
                Mdi = mdi,
                Dx = dx.NaN2Null(),
                Adx = adx.NaN2Null(),
                Adxr = adxr.NaN2Null()
            });
        }

        return results;
    }

    // parameter validation
    private static void ValidateAdx(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for ADX.");
        }
    }
}
