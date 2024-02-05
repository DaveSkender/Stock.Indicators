namespace Skender.Stock.Indicators;

// KELTNER CHANNELS (SERIES)

public static partial class Indicator
{
    internal static List<KeltnerResult> CalcKeltner(
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
            .ToTuple(CandlePart.Close)
            .CalcEma(emaPeriods)
            .ToList();

        List<AtrResult> atrResults = qdList
            .CalcAtr(atrPeriods)
            .ToList();

        int lookbackPeriods = Math.Max(emaPeriods, atrPeriods);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            QuoteD q = qdList[i];

            KeltnerResult r = new() { Timestamp = q.Timestamp };
            results.Add(r);

            if (i + 1 >= lookbackPeriods)
            {
                EmaResult ema = emaResults[i];
                AtrResult atr = atrResults[i];
                double? atrSpan = atr.Atr * multiplier;

                r.UpperBand = ema.Ema + atrSpan;
                r.LowerBand = ema.Ema - atrSpan;
                r.Centerline = ema.Ema;
                r.Width = (r.Centerline == 0) ? null
                    : (r.UpperBand - r.LowerBand) / r.Centerline;
            }
        }

        return results;
    }
}
