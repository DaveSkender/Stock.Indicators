namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // STARC BANDS
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<StarcBandsResult> GetStarcBands<TQuote>(
        this IEnumerable<TQuote> quotes,
        int smaPeriods = 20,
        decimal multiplier = 2,
        int atrPeriods = 10)
        where TQuote : IQuote
    {
        // check parameter arguments
        ValidateStarcBands(smaPeriods, multiplier, atrPeriods);

        // initialize
        List<AtrResult> atrResults = GetAtr(quotes, atrPeriods).ToList();
        List<StarcBandsResult> results = GetSma(quotes, smaPeriods)
            .Select(x => new StarcBandsResult
            {
                Date = x.Date,
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
        decimal multiplier,
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
