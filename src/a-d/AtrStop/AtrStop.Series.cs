namespace Skender.Stock.Indicators;

// ATR TRAILING STOP (SERIES)

public static partial class AtrStop
{
    private static List<AtrStopResult> CalcAtrStop(
        this List<QuoteD> source,
        int lookbackPeriods,
        double multiplier,
        EndType endType)
    {
        // check parameter arguments
        Validate(lookbackPeriods, multiplier);

        // initialize
        int length = source.Count;
        List<AtrStopResult> results = new(length);
        List<AtrResult> atrResults = source.CalcAtr(lookbackPeriods);

        // prevailing direction and bands
        bool isBullish = true;
        double? upperBand = null;
        double? lowerBand = null;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = source[i];

            decimal? atrStop = null;
            decimal? buyStop = null;
            decimal? sellStop = null;

            if (i >= lookbackPeriods)
            {
                double? atr = atrResults[i].Atr;
                QuoteD p = source[i - 1];

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

                // initialize values on first eval pass
                if (i == lookbackPeriods)
                {
                    isBullish = q.Close >= p.Close;

                    upperBand = upperEval;
                    lowerBand = lowerEval;
                }

                // new upper band: can only go down, or reverse
                if (upperEval < upperBand || p.Close > upperBand)
                {
                    upperBand = upperEval;
                }

                // new lower band: can only go up, or reverse
                if (lowerEval > lowerBand || p.Close < lowerBand)
                {
                    lowerBand = lowerEval;
                }

                // trailing stop: based on direction,
                // can be either the upper or lower band
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

            results.Add(new AtrStopResult(
                Timestamp: q.Timestamp,
                AtrStop: atrStop,
                BuyStop: buyStop,
                SellStop: sellStop));
        }

        return results;
    }
}
