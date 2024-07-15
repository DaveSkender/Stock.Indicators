namespace Skender.Stock.Indicators;

// ATR TRAILING STOP (SERIES)

public static partial class Indicator
{
    private static List<AtrStopResult> CalcAtrStop(
        this List<QuoteD> qdList,
        int lookbackPeriods,
        double multiplier,
        EndType endType)
    {
        // check parameter arguments
        AtrStop.Validate(lookbackPeriods, multiplier);

        // initialize
        int length = qdList.Count;
        List<AtrStopResult> results = new(length);
        List<AtrResult> atrResults = qdList.CalcAtr(lookbackPeriods);

        bool isBullish = true;
        double? upperBand = null;
        double? lowerBand = null;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = qdList[i];

            decimal? atrStop = null;
            decimal? buyStop = null;
            decimal? sellStop = null;

            if (i >= lookbackPeriods)
            {
                double? atr = atrResults[i].Atr;
                QuoteD p = qdList[i - 1];

                double? upperEval;
                double? lowerEval;

                // potential bands for CLOSE
                if (endType == EndType.Close)
                {
                    upperEval = q.Close + multiplier * atr;
                    lowerEval = q.Close - multiplier * atr;
                }

                // potential bands for HIGH/LOW
                else
                {
                    upperEval = q.High + multiplier * atr;
                    lowerEval = q.Low - multiplier * atr;
                }

                // initial values
                // TODO: update healing, without requiring specific indexing
                if (i == lookbackPeriods)
                {
                    isBullish = q.Close >= p.Close;

                    upperBand = upperEval;
                    lowerBand = lowerEval;
                }

                // new upper band
                if (upperEval < upperBand || p.Close > upperBand)
                {
                    upperBand = upperEval;
                }

                // new lower band
                if (lowerEval > lowerBand || p.Close < lowerBand)
                {
                    lowerBand = lowerEval;
                }

                // trailing stop
                if (q.Close <= (isBullish ? lowerBand : upperBand))
                {
                    atrStop = (decimal?)upperBand;
                    buyStop = (decimal?)upperBand;
                    isBullish = false;
                }
                else
                {
                    atrStop = (decimal?)lowerBand;
                    sellStop = (decimal?)lowerBand;
                    isBullish = true;
                }
            }

            AtrStopResult r = new(
                Timestamp: q.Timestamp,
                AtrStop: atrStop,
                BuyStop: buyStop,
                SellStop: sellStop);

            results.Add(r);
        }

        return results;
    }
}
