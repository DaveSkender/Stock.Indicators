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
        double prevTrs = 0; // smoothed
        double prevPdm = 0;
        double prevMdm = 0;
        double prevAdx = 0;

        double sumTr = 0;
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
                prevClose = q.Close;
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
                continue;
            }

            // smoothed true range and directional movement
            double trs;
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

            if (trs is 0)
            {
                continue;
            }

            // directional increments
            double pdi = 100 * pdm / trs;
            double mdi = 100 * mdm / trs;

            r.Pdi = pdi;
            r.Mdi = mdi;

            // calculate ADX
            double dx = (pdi == mdi)
                ? 0
                : (pdi + mdi != 0)
                ? 100 * Math.Abs(pdi - mdi) / (pdi + mdi)
                : double.NaN;

            double adx;

            if (i > (2 * lookbackPeriods) - 1)
            {
                adx = ((prevAdx * (lookbackPeriods - 1)) + dx) / lookbackPeriods;
                r.Adx = adx.NaN2Null();

                double? priorAdx = results[i + 1 - lookbackPeriods].Adx;

                r.Adxr = (adx + priorAdx).NaN2Null() / 2;
                prevAdx = adx;
            }

            // initial ADX
            else if (i == (2 * lookbackPeriods) - 1)
            {
                sumDx += dx;
                adx = sumDx / lookbackPeriods;
                r.Adx = adx.NaN2Null();
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
