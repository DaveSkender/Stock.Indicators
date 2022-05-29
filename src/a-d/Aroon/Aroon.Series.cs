namespace Skender.Stock.Indicators;

// AROON OSCILLATOR (SERIES)
public static partial class Indicator
{
    internal static IEnumerable<AroonResult> CalcAroon(
        this List<QuoteD> qdList,
        int lookbackPeriods = 25)
    {
        // check parameter arguments
        ValidateAroon(lookbackPeriods);

        // initialize
        List<AroonResult> results = new(qdList.Count);

        // roll through quotes
        for (int i = 0; i < qdList.Count; i++)
        {
            QuoteD q = qdList[i];

            AroonResult result = new()
            {
                Date = q.Date
            };

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

                result.AroonUp = 100d * (lookbackPeriods - (i + 1 - lastHighIndex)) / lookbackPeriods;
                result.AroonDown = 100d * (lookbackPeriods - (i + 1 - lastLowIndex)) / lookbackPeriods;
                result.Oscillator = result.AroonUp - result.AroonDown;
            }

            results.Add(result);
        }

        return results;
    }

    // parameter validation
    private static void ValidateAroon(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Aroon.");
        }
    }
}
