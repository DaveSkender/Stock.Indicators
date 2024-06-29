namespace Skender.Stock.Indicators;

// ELDER-RAY (SERIES)

public static partial class Indicator
{
    internal static List<ElderRayResult> CalcElderRay(
        this List<QuoteD> qdList,
        int lookbackPeriods)
    {
        // check parameter arguments
        ElderRay.Validate(lookbackPeriods);

        // initialize
        int length = qdList.Count;
        List<ElderRayResult> results = new(length);

        // EMA
        List<EmaResult> emaResults
            = qdList.CalcEma(lookbackPeriods);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            QuoteD q = qdList[i];
            EmaResult e = emaResults[i];

            results.Add(new ElderRayResult(
                Timestamp: e.Timestamp,
                Ema: e.Ema,
                BullPower: q.High - e.Ema,
                BearPower: q.Low - e.Ema));
        }

        return results;
    }
}
