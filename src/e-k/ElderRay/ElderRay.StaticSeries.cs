namespace Skender.Stock.Indicators;

// ELDER-RAY (SERIES)

public static partial class Indicator
{
    private static List<ElderRayResult> CalcElderRay(
        this List<QuoteD> source,
        int lookbackPeriods)
    {
        // check parameter arguments
        ElderRay.Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<ElderRayResult> results = new(length);

        // EMA
        IReadOnlyList<EmaResult> emaResults
            = source.ToEma(lookbackPeriods);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = source[i];
            EmaResult e = emaResults[i];

            results.Add(new(
                Timestamp: e.Timestamp,
                Ema: e.Ema,
                BullPower: q.High - e.Ema,
                BearPower: q.Low - e.Ema));
        }

        return results;
    }
}
