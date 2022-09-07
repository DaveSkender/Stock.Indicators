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
        ValidateAtrStop(lookbackPeriods, multiplier);

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

            AtrStopResult r = new(q.Date);
            results.Add(r);

            if (i >= lookbackPeriods - 1)
            {
                double? atr = atrResults[i].Atr;
                QuoteD p = qdList[i - 1];

                // potential bands
                double? upperEval = q.Close + (multiplier * atr);
                double? lowerEval = q.Close - (multiplier * atr);

                // initial values
                if (i == lookbackPeriods - 1)
                {
                    isBullish = q.Close >= p.Close;

                    upperBand = upperEval;
                    lowerBand = lowerEval;
                }

                // USING CLOSE TRIGGER
                if (endType == EndType.Close)
                {
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

                //// USING HIGH/LOW TRIGGER
                //else
                //{
                //    // new upper band
                //    if (upperEval < upperBand || p.High > upperBand)
                //    {
                //        upperBand = upperEval;
                //    }

                //    // new lower band
                //    if (lowerEval > lowerBand || p.Low < lowerBand)
                //    {
                //        lowerBand = lowerEval;
                //    }

                //    // trailing stop
                //    if (q.Low <= (isBullish ? lowerBand : upperBand))
                //    {
                //        r.AtrStop = (decimal?)upperBand;
                //        r.BuyStop = (decimal?)upperBand;
                //        isBullish = false;
                //    }
                //    else
                //    {
                //        r.AtrStop = (decimal?)lowerBand;
                //        r.SellStop = (decimal?)lowerBand;
                //        isBullish = true;
                //    }
                //}
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateAtrStop(
        int lookbackPeriods,
        double multiplier)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for ATR Trailing Stop.");
        }

        if (multiplier <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(multiplier), multiplier,
                "Multiplier must be greater than 0 for ATR Trailing Stop.");
        }
    }
}
