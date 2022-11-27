namespace Skender.Stock.Indicators;

// SUPERTREND (SERIES)
public static partial class Indicator
{
    internal static List<SuperTrendResult> CalcSuperTrend(
        this List<QuoteD> qdList,
        int lookbackPeriods,
        double multiplier)
    {
        // check parameter arguments
        ValidateSuperTrend(lookbackPeriods, multiplier);

        // initialize
        List<SuperTrendResult> results = new(qdList.Count);
        List<AtrResult> atrResults = qdList.CalcAtr(lookbackPeriods);

        bool isBullish = true;
        double? upperBand = null;
        double? lowerBand = null;

        // roll through quotes
        for (int i = 0; i < qdList.Count; i++)
        {
            QuoteD q = qdList[i];

            SuperTrendResult r = new(q.Date);
            results.Add(r);

            if (i >= lookbackPeriods)
            {
                double? mid = (q.High + q.Low) / 2;
                double? atr = atrResults[i].Atr;
                double? prevClose = qdList[i - 1].Close;

                // potential bands
                double? upperEval = mid + (multiplier * atr);
                double? lowerEval = mid - (multiplier * atr);

                // initial values
                if (i == lookbackPeriods)
                {
                    isBullish = q.Close >= mid;

                    upperBand = upperEval;
                    lowerBand = lowerEval;
                }

                // new upper band
                if (upperEval < upperBand || prevClose > upperBand)
                {
                    upperBand = upperEval;
                }

                // new lower band
                if (lowerEval > lowerBand || prevClose < lowerBand)
                {
                    lowerBand = lowerEval;
                }

                // supertrend
                if (q.Close <= (isBullish ? lowerBand : upperBand))
                {
                    r.SuperTrend = (decimal?)upperBand;
                    r.UpperBand = (decimal?)upperBand;
                    isBullish = false;
                }
                else
                {
                    r.SuperTrend = (decimal?)lowerBand;
                    r.LowerBand = (decimal?)lowerBand;
                    isBullish = true;
                }
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateSuperTrend(
        int lookbackPeriods,
        double multiplier)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for SuperTrend.");
        }

        if (multiplier <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(multiplier), multiplier,
                "Multiplier must be greater than 0 for SuperTrend.");
        }
    }
}
