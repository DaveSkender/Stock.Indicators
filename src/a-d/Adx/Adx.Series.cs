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
        List<AtrResult> atr = qdList
            .CalcAtr(lookbackPeriods)
            .ToList();

        double prevHigh = 0;
        double prevLow = 0;
        double? prevTrs = 0; // smoothed
        double prevPdm = 0;
        double prevMdm = 0;
        double prevAdx = 0;

        double? sumTr = 0;
        double sumPdm = 0;
        double sumMdm = 0;
        double sumDx = 0;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            QuoteD q = qdList[i];

            AdxResult r = new(q.Date);
            results.Add(r);

            // skip first period
            if (i == 0)
            {
                prevHigh = q.High;
                prevLow = q.Low;
                continue;
            }

            double? tr = atr[i].Tr;

            double pdm1 = (q.High - prevHigh) > (prevLow - q.Low) ?
                Math.Max(q.High - prevHigh, 0) : 0;

            double mdm1 = (prevLow - q.Low) > (q.High - prevHigh) ?
                Math.Max(prevLow - q.Low, 0) : 0;

            prevHigh = q.High;
            prevLow = q.Low;

            // initialization period
            if (i <= lookbackPeriods)
            {
                sumTr += tr;
                sumPdm += pdm1;
                sumMdm += mdm1;
            }

            // skip DM initialization period
            if (i + 1 <= lookbackPeriods)
            {
                continue;
            }

            // smoothed true range and directional movement
            double? trs;
            double pdm;
            double mdm;

            if (i == lookbackPeriods)
            {
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

            if (trs is 0 or null)
            {
                continue;
            }

            // directional increments
            double pdi = 100 * pdm / (double)trs;
            double mdi = 100 * mdm / (double)trs;

            r.Pdi = pdi;
            r.Mdi = mdi;

            // calculate ADX
            double dx = (pdi + mdi == 0)
                ? double.NaN
                : 100 * Math.Abs(pdi - mdi) / (pdi + mdi);
            double adx;

            if (i + 1 > 2 * lookbackPeriods)
            {
                adx = ((prevAdx * (lookbackPeriods - 1)) + dx) / lookbackPeriods;
                r.Adx = adx;

                double? priorAdx = results[i + 1 - lookbackPeriods].Adx;

                r.Adxr = (adx + priorAdx) / 2;
                prevAdx = adx;
            }

            // initial ADX
            else if (i + 1 == 2 * lookbackPeriods)
            {
                sumDx += dx;
                adx = sumDx / lookbackPeriods;
                r.Adx = adx;
                prevAdx = adx;
            }

            // ADX initialization period
            else
            {
                sumDx += dx;
            }
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
