namespace Skender.Stock.Indicators;

// ELDER-RAY (SERIES)
public static partial class Indicator
{
    internal static List<ElderRayResult> CalcElderRay(
        this List<QuoteD> qdList,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateElderRay(lookbackPeriods);

        // initialize with EMA
        List<ElderRayResult> results = qdList
            .ToBasicTuple(CandlePart.Close)
            .CalcEma(lookbackPeriods)
            .Select(x => new ElderRayResult(x.Date)
            {
                Ema = x.Ema
            })
            .ToList();

        // roll through quotes
        for (int i = lookbackPeriods - 1; i < qdList.Count; i++)
        {
            QuoteD q = qdList[i];
            ElderRayResult r = results[i];

            r.BullPower = q.High - r.Ema;
            r.BearPower = q.Low - r.Ema;
        }

        return results;
    }

    // parameter validation
    private static void ValidateElderRay(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Elder-ray Index.");
        }
    }
}
