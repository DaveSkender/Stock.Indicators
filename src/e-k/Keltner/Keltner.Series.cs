namespace Skender.Stock.Indicators;

// KELTNER CHANNELS (SERIES)

public static partial class Indicator
{
    private static List<KeltnerResult> CalcKeltner(
        this List<QuoteD> qdList,
        int emaPeriods,
        double multiplier,
        int atrPeriods)
    {
        // check parameter arguments
        Keltner.Validate(emaPeriods, multiplier, atrPeriods);

        // initialize
        int length = qdList.Count;
        List<KeltnerResult> results = new(length);

        List<EmaResult> emaResults = qdList
            .CalcEma(emaPeriods)
            .ToList();

        List<AtrResult> atrResults = qdList
            .CalcAtr(atrPeriods)
            .ToList();

        int lookbackPeriods = Math.Max(emaPeriods, atrPeriods);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = qdList[i];

            if (i >= lookbackPeriods - 1)
            {
                EmaResult ema = emaResults[i];
                AtrResult atr = atrResults[i];
                double? atrSpan = atr.Atr * multiplier;

                results.Add(new KeltnerResult(
                    Timestamp: q.Timestamp,
                    UpperBand: ema.Ema + atrSpan,
                    LowerBand: ema.Ema - atrSpan,
                    Centerline: ema.Ema,
                    Width: ema.Ema == 0 ? null : 2 * atrSpan / ema.Ema));
            }
            else
            {
                results.Add(new(q.Timestamp));
            }
        }

        return results;
    }
}
