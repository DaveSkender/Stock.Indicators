namespace Skender.Stock.Indicators;

// STARC BANDS (SERIES)

public static partial class Indicator
{
    internal static List<StarcBandsResult> CalcStarcBands(
        this List<QuoteD> qdList,
        int smaPeriods,
        double multiplier,
        int atrPeriods)
    {
        // check parameter arguments
        StarcBands.Validate(smaPeriods, multiplier, atrPeriods);

        // initialize
        List<AtrResult> atrResults = qdList.CalcAtr(atrPeriods);

        List<StarcBandsResult> results = qdList
            .ToTuple(CandlePart.Close)
            .CalcSma(smaPeriods)
            .Select(x => new StarcBandsResult
            {
                TickDate = x.TickDate,
                Centerline = x.Sma
            })
            .ToList();

        int lookbackPeriods = Math.Max(smaPeriods, atrPeriods);

        // roll through quotes
        for (int i = lookbackPeriods - 1; i < results.Count; i++)
        {
            StarcBandsResult r = results[i];

            AtrResult a = atrResults[i];

            r.UpperBand = r.Centerline + (multiplier * a.Atr);
            r.LowerBand = r.Centerline - (multiplier * a.Atr);
        }

        return results;
    }
}
