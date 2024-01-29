namespace Skender.Stock.Indicators;

// AROON OSCILLATOR (SERIES)

public static partial class Indicator
{
    internal static List<AroonResult> CalcAroon(
        this List<QuoteD> qdList,
        int lookbackPeriods)
    {
        // check parameter arguments
        Aroon.Validate(lookbackPeriods);

        // initialize
        List<AroonResult> results = new(qdList.Count);

        // roll through quotes
        for (int i = 0; i < qdList.Count; i++)
        {
            QuoteD q = qdList[i];

            AroonResult r = new() { TickDate = q.TickDate };
            results.Add(r);

            // add aroons
            if (i + 1 > lookbackPeriods)
            {
                double? lastHighPrice = 0;
                double? lastLowPrice = double.MaxValue;
                int lastHighIndex = 0;
                int lastLowIndex = 0;

                for (int p = i + 1 - lookbackPeriods - 1; p <= i; p++)
                {
                    QuoteD d = qdList[p];

                    if (d.High > lastHighPrice)
                    {
                        lastHighPrice = d.High;
                        lastHighIndex = p + 1;
                    }

                    if (d.Low < lastLowPrice)
                    {
                        lastLowPrice = d.Low;
                        lastLowIndex = p + 1;
                    }
                }

                r.AroonUp = 100d * (lookbackPeriods - (i + 1 - lastHighIndex)) / lookbackPeriods;
                r.AroonDown = 100d * (lookbackPeriods - (i + 1 - lastLowIndex)) / lookbackPeriods;
                r.Oscillator = r.AroonUp - r.AroonDown;
            }
        }

        return results;
    }
}
