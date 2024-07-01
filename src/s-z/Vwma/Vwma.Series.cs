namespace Skender.Stock.Indicators;

// VOLUME WEIGHTED MOVING AVERAGE (SERIES)

public static partial class Indicator
{
    private static List<VwmaResult> CalcVwma(
        this List<QuoteD> qdList,
        int lookbackPeriods)
    {
        // check parameter arguments
        Vwma.Validate(lookbackPeriods);

        // initialize
        int length = qdList.Count;
        List<VwmaResult> results = new(length);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            QuoteD q = qdList[i];

            double vwma;

            if (i + 1 >= lookbackPeriods)
            {
                double sumCl = 0;
                double sumVl = 0;
                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    QuoteD d = qdList[p];
                    double c = d.Close;
                    double v = d.Volume;

                    sumCl += c * v;
                    sumVl += v;
                }

                vwma = sumVl != 0 ? sumCl / sumVl : double.NaN;
            }
            else
            {
                vwma = double.NaN;
            }

            results.Add(new(
                Timestamp: q.Timestamp,
                Vwma: vwma.NaN2Null()));
        }

        return results;
    }
}
