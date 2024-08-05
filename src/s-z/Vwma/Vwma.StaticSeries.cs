namespace Skender.Stock.Indicators;

// VOLUME WEIGHTED MOVING AVERAGE (SERIES)

public static partial class Indicator
{
    private static List<VwmaResult> CalcVwma(
        this List<QuoteD> source,
        int lookbackPeriods)
    {
        // check parameter arguments
        Vwma.Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<VwmaResult> results = new(length);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = source[i];

            double vwma;

            if (i + 1 >= lookbackPeriods)
            {
                double sumCl = 0;
                double sumVl = 0;
                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    QuoteD d = source[p];
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
