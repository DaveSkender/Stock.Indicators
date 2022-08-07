namespace Skender.Stock.Indicators;

// AVERAGE TRUE RANGE (SERIES)
public static partial class Indicator
{
    // calculate series
    internal static List<AtrResult> CalcAtr(
        this List<QuoteD> qdList,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateAtr(lookbackPeriods);

        // initialize
        List<AtrResult> results = new(qdList.Count);
        double prevAtr = 0;
        double prevClose = 0;
        double highMinusPrevClose = 0;
        double lowMinusPrevClose = 0;
        double sumTr = 0;

        // roll through quotes
        for (int i = 0; i < qdList.Count; i++)
        {
            QuoteD q = qdList[i];

            AtrResult r = new(q.Date);
            results.Add(r);

            if (i > 0)
            {
                highMinusPrevClose = Math.Abs(q.High - prevClose);
                lowMinusPrevClose = Math.Abs(q.Low - prevClose);
            }

            double tr = Math.Max(q.High - q.Low, Math.Max(highMinusPrevClose, lowMinusPrevClose));
            r.Tr = tr;

            if (i + 1 > lookbackPeriods)
            {
                // calculate ATR
                double atr = ((prevAtr * (lookbackPeriods - 1)) + tr) / lookbackPeriods;
                r.Atr = atr;
                r.Atrp = (q.Close == 0) ? null : atr / q.Close * 100;
                prevAtr = atr;
            }
            else if (i + 1 == lookbackPeriods)
            {
                // initialize ATR
                sumTr += tr;
                double atr = sumTr / lookbackPeriods;
                r.Atr = atr;
                r.Atrp = (q.Close == 0) ? null : atr / q.Close * 100;
                prevAtr = atr;
            }
            else
            {
                // only used for periods before ATR initialization
                sumTr += tr;
            }

            prevClose = q.Close;
        }

        return results;
    }

    // parameter validation
    private static void ValidateAtr(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for Average True Range.");
        }
    }
}
