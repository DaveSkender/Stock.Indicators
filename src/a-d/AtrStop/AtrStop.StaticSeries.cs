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
        double upperBand = double.MaxValue;
        double lowerBand = double.MinValue;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            // handle warmup periods
            if (i < lookbackPeriods)
            {
                results.Add(new(Timestamp: source[i].Timestamp));
                continue;
            }

            QuoteD q = source[i];
            QuoteD p = source[i - 1];

            // initialize direction on first evaluation
            if (i == lookbackPeriods)
            {
                isBullish = q.Close >= p.Close;
            }

            // evaluate bands
            double upperEval;
            double lowerEval;
            double atr = atrResults[i].Atr ?? double.NaN;

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

            // trailing stop: based on direction

            AtrStopResult r;

            // the upper band (short / buy-to-stop)
            if (q.Close <= (isBullish ? lowerBand : upperBand))
            {
                isBullish = false;

                r = new(
                    Timestamp: q.Timestamp,
                    AtrStop: upperBand,
                    BuyStop: upperBand,
                    SellStop: null,
                    Atr: atr);
            }

            // the lower band (long / sell-to-stop)
            else
            {
                isBullish = true;

                r = new(
                    Timestamp: q.Timestamp,
                    AtrStop: lowerBand,
                    BuyStop: null,
                    SellStop: lowerBand,
                    Atr: atr);
            }

            results.Add(r);
        }

        return results;
    }
}
