namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Average Directional Index (ADX).
/// </summary>
public static partial class Adx
{
    /// <summary>
    /// Calculates the Average Directional Index (ADX) from a series of quotes.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
    /// <param name="quotes">The list of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the ADX calculation.</param>
    /// <returns>A list of ADX results.</returns>
    [Series("ADX", "Average Directional Index (ADX)", Category.PriceTrend, ChartType.Oscillator)]
    public static IReadOnlyList<AdxResult> ToAdx<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        [ParamNum<int>("Lookback Periods", 14, 2, 250)]
        int lookbackPeriods = 14)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcAdx(lookbackPeriods);

    /// <summary>
    /// Calculates the ADX from a list of quotes.
    /// </summary>
    /// <param name="source">The list of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the ADX calculation.</param>
    /// <returns>A list of ADX results.</returns>
    private static List<AdxResult> CalcAdx(
        this List<QuoteD> source,
        int lookbackPeriods = 14)
    {
        // check parameter arguments
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
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

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = source[i];

            // skip first period
            if (i == 0)
            {
                prevHigh = q.High;
                prevLow = q.Low;
                prevClose = q.Close;

                results.Add(new(Timestamp: q.Timestamp));
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
                results.Add(new(Timestamp: q.Timestamp));
                continue;
            }

            // smoothed true range and directional movement
            double trs;
            double pdm;
            double mdm;

            // TODO: update healing, without requiring specific indexing
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

            if (trs == 0)
            {
                results.Add(new(Timestamp: q.Timestamp));
                continue;
            }

            // directional increments
            double pdi = 100 * pdm / trs;
            double mdi = 100 * mdm / trs;

            // calculate ADX
            double dx = pdi - mdi == 0
                ? 0
                : pdi + mdi != 0
                ? 100 * Math.Abs(pdi - mdi) / (pdi + mdi)
                : double.NaN;

            double adx = double.NaN;
            double adxr = double.NaN;

            if (i > (2 * lookbackPeriods) - 1)
            {
                adx = ((prevAdx * (lookbackPeriods - 1)) + dx) / lookbackPeriods;

                double priorAdx = results[i - lookbackPeriods + 1].Adx.Null2NaN();

                adxr = (adx + priorAdx) / 2;
                prevAdx = adx;
            }

            // initial ADX
            else if (i == (2 * lookbackPeriods) - 1)
            {
                sumDx += dx;
                adx = sumDx / lookbackPeriods;
                prevAdx = adx;
            }

            // ADX initialization period
            // TODO: update healing, without requiring specific indexing
            //       see ADX BufferList for hint
            else
            {
                sumDx += dx;
            }

            AdxResult r = new(
                Timestamp: q.Timestamp,
                Pdi: pdi,
                Mdi: mdi,
                Dx: dx.NaN2Null(),
                Adx: adx.NaN2Null(),
                Adxr: adxr.NaN2Null());

            results.Add(r);
        }

        return results;
    }
}
