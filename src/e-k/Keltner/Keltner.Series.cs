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
        ValidateKeltner(emaPeriods, multiplier, atrPeriods);

        // initialize
        int length = qdList.Count;
        List<KeltnerResult> results = new(length);

        List<EmaResult> emaResults = qdList
            .ToBasicTuple(CandlePart.Close)
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

            KeltnerResult result = new()
            {
                Date = q.Date
            };

            if (i + 1 >= lookbackPeriods)
            {
                EmaResult ema = emaResults[i];
                AtrResult atr = atrResults[i];
                double? atrSpan = atr.Atr * multiplier;

                result.UpperBand = ema.Ema + atrSpan;
                result.LowerBand = ema.Ema - atrSpan;
                result.Centerline = ema.Ema;
                result.Width = (result.Centerline == 0) ? null
                    : (result.UpperBand - result.LowerBand) / result.Centerline;
            }

            results.Add(result);
        }

        return results;
    }

    // parameter validation
    private static void ValidateKeltner(
        int emaPeriods,
        double multiplier,
        int atrPeriods)
    {
        // check parameter arguments
        if (emaPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(emaPeriods), emaPeriods,
                "EMA periods must be greater than 1 for Keltner Channel.");
        }

        if (atrPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(atrPeriods), atrPeriods,
                "ATR periods must be greater than 1 for Keltner Channel.");
        }

        if (multiplier <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(multiplier), multiplier,
                "Multiplier must be greater than 0 for Keltner Channel.");
        }
    }
}
