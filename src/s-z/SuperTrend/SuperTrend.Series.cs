namespace Skender.Stock.Indicators;

// SUPERTREND (SERIES)

public static partial class Indicator
{
    private static List<SuperTrendResult> CalcSuperTrend(
        this List<QuoteD> qdList,
        int lookbackPeriods,
        double multiplier)
    {
        // check parameter arguments
        SuperTrend.Validate(lookbackPeriods, multiplier);

        // initialize
        int length = qdList.Count;
        List<SuperTrendResult> results = new(length);
        List<AtrResult> atrResults = qdList.CalcAtr(lookbackPeriods);

        bool isBullish = true;
        double? upperBand = null;
        double? lowerBand = null;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = qdList[i];

            double? superTrend;
            double? upperOnly;
            double? lowerOnly;

            if (i >= lookbackPeriods)
            {
                double? mid = (q.High + q.Low) / 2;
                double? atr = atrResults[i].Atr;
                double? prevClose = qdList[i - 1].Close;

                // potential bands
                double? upperEval = mid + multiplier * atr;
                double? lowerEval = mid - multiplier * atr;

                // initial values
                // TODO: update healing, without requiring specific indexing
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
                    superTrend = upperBand;
                    upperOnly = upperBand;
                    lowerOnly = null;
                    isBullish = false;
                }
                else
                {
                    superTrend = lowerBand;
                    lowerOnly = lowerBand;
                    upperOnly = null;
                    isBullish = true;
                }
            }
            else
            {
                superTrend = null;
                upperOnly = null;
                lowerOnly = null;
            }

            results.Add(new(
                Timestamp: q.Timestamp,
                SuperTrend: (decimal?)superTrend,
                UpperBand: (decimal?)upperOnly,
                LowerBand: (decimal?)lowerOnly));
        }

        return results;
    }
}
