using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// ELDER-RAY (SERIES)
public static partial class Indicator
{
    internal static Collection<ElderRayResult> CalcElderRay(
        this Collection<QuoteD> qdList,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateElderRay(lookbackPeriods);

        // initialize with EMA
        Collection<ElderRayResult> results = new();

        Collection<EmaResult> ema = qdList
            .ToTuple(CandlePart.Close)
            .CalcEma(lookbackPeriods);

        foreach (EmaResult r in ema)
        {
            results.Add(new ElderRayResult(r.Date) { Ema = r.Ema });
        }

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
