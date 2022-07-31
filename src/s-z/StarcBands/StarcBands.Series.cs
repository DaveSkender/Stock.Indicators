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
        ValidateStarcBands(smaPeriods, multiplier, atrPeriods);

        // initialize
        List<AtrResult> atrResults = qdList.CalcAtr(atrPeriods);

        List<StarcBandsResult> results = qdList
            .ToBasicTuple(CandlePart.Close)
            .CalcSma(smaPeriods)
            .Select(x => new StarcBandsResult(x.Date)
            {
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

    // parameter validation
    private static void ValidateStarcBands(
        int smaPeriods,
        double multiplier,
        int atrPeriods)
    {
        // check parameter arguments
        if (smaPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(smaPeriods), smaPeriods,
                "EMA periods must be greater than 1 for STARC Bands.");
        }

        if (atrPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(atrPeriods), atrPeriods,
                "ATR periods must be greater than 1 for STARC Bands.");
        }

        if (multiplier <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(multiplier), multiplier,
                "Multiplier must be greater than 0 for STARC Bands.");
        }
    }
}
