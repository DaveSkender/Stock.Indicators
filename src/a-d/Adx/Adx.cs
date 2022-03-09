namespace Skender.Stock.Indicators;
#nullable disable

public static partial class Indicator
{
    // AVERAGE DIRECTIONAL INDEX
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<AdxResult> GetAdx<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 14)
        where TQuote : IQuote
    {
        // convert quotes
        List<QuoteD> quotesList = quotes.ConvertToList();

        // check parameter arguments
        ValidateAdx(lookbackPeriods);

        // initialize
        List<AdxResult> results = new(quotesList.Count);
        List<AtrResult> atr = GetAtr(quotes, lookbackPeriods).ToList(); // get True Range info

        double prevHigh = 0;
        double prevLow = 0;
        double prevTrs = 0; // smoothed
        double prevPdm = 0;
        double prevMdm = 0;
        double prevAdx = 0;

        double sumTr = 0;
        double sumPdm = 0;
        double sumMdm = 0;
        double sumDx = 0;

        // roll through quotes
        for (int i = 0; i < quotesList.Count; i++)
        {
            QuoteD q = quotesList[i];
            int index = i + 1;

            AdxResult result = new()
            {
                Date = q.Date
            };
            results.Add(result);

            // skip first period
            if (index == 1)
            {
                prevHigh = q.High;
                prevLow = q.Low;
                continue;
            }

            double tr = (double)atr[i].Tr;

            double pdm1 = (q.High - prevHigh) > (prevLow - q.Low) ?
                Math.Max(q.High - prevHigh, 0) : 0;

            double mdm1 = (prevLow - q.Low) > (q.High - prevHigh) ?
                Math.Max(prevLow - q.Low, 0) : 0;

            prevHigh = q.High;
            prevLow = q.Low;

            // initialization period
            if (index <= lookbackPeriods + 1)
            {
                sumTr += tr;
                sumPdm += pdm1;
                sumMdm += mdm1;
            }

            // skip DM initialization period
            if (index <= lookbackPeriods)
            {
                continue;
            }

            // smoothed true range and directional movement
            double trs;
            double pdm;
            double mdm;

            if (index == lookbackPeriods + 1)
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

            if (trs == 0)
            {
                continue;
            }

            // directional increments
            double pdi = 100 * pdm / trs;
            double mdi = 100 * mdm / trs;

            result.Pdi = pdi;
            result.Mdi = mdi;

            if (pdi + mdi == 0)
            {
                continue;
            }

            // calculate ADX
            double dx = 100 * Math.Abs(pdi - mdi) / (pdi + mdi);
            double adx;

            if (index > 2 * lookbackPeriods)
            {
                adx = ((prevAdx * (lookbackPeriods - 1)) + dx) / lookbackPeriods;
                result.Adx = adx;

                double? priorAdx = results[index - lookbackPeriods].Adx;

                result.Adxr = (adx + priorAdx) / 2;
                prevAdx = adx;
            }

            // initial ADX
            else if (index == 2 * lookbackPeriods)
            {
                sumDx += dx;
                adx = sumDx / lookbackPeriods;
                result.Adx = adx;
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
