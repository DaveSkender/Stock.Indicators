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

        // initialize with EMA
        List<ElderRayResult> results = qdList
            .ToTuple(CandlePart.Close)
            .CalcEma(lookbackPeriods)
            .Select(x => new ElderRayResult()
            {
                Date = x.Date,
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
}
