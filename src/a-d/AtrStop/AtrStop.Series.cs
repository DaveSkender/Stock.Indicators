namespace Skender.Stock.Indicators;

// ATR TRAILING STOP (SERIES)

public static partial class Indicator
{
    internal static List<AtrStopResult> CalcAtrStop(
        this List<QuoteD> qdList,
        int lookbackPeriods,
        double multiplier,
        EndType endType)
    {
        // check parameter arguments
        AtrStop.Validate(lookbackPeriods, multiplier);

        // initialize
        List<AtrStopResult> results = new(qdList.Count);
        List<AtrResult> atrResults = qdList.CalcAtr(lookbackPeriods);

        bool isBullish = true;
        double? upperBand = null;
        double? lowerBand = null;

        // roll through quotes
        for (int i = 0; i < qdList.Count; i++)
        {
            QuoteD q = qdList[i];

            AtrStopResult r = new() { Date = q.Date };
            results.Add(r);

            if (i >= lookbackPeriods)
            {
                double? atr = atrResults[i].Atr;
                QuoteD p = qdList[i - 1];

                double? upperEval;
                double? lowerEval;

                // potential bands for CLOSE
                if (endType == EndType.Close)
                {
                    upperEval = q.Close + (multiplier * atr);
                    lowerEval = q.Close - (multiplier * atr);
                }

                // potential bands for HIGH/LOW
                else
                {
                    upperEval = q.High + (multiplier * atr);
                    lowerEval = q.Low - (multiplier * atr);
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
                    r.AtrStop = (decimal?)upperBand;
                    r.BuyStop = (decimal?)upperBand;
                    isBullish = false;
                }
                else
                {
                    r.AtrStop = (decimal?)lowerBand;
                    r.SellStop = (decimal?)lowerBand;
                    isBullish = true;
                }
            }
        }

        return results;
    }
}
