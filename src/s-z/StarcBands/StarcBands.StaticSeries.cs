namespace Skender.Stock.Indicators;

// STARC BANDS (SERIES)

public static partial class StarcBands
{
    public static IReadOnlyList<StarcBandsResult> ToStarcBands<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int smaPeriods,
        double multiplier = 2,
        int atrPeriods = 10)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcStarcBands(smaPeriods, multiplier, atrPeriods);

    private static List<StarcBandsResult> CalcStarcBands(
        this IReadOnlyList<QuoteD> source,
        int smaPeriods,
        double multiplier,
        int atrPeriods)
    {
        // check parameter arguments
        Validate(smaPeriods, multiplier, atrPeriods);

        // initialize
        int length = source.Count;
        List<StarcBandsResult> results = new(length);
        List<AtrResult> atrResults = source.CalcAtr(atrPeriods);
        IReadOnlyList<SmaResult> smaResults = source.ToSma(smaPeriods);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            SmaResult s = smaResults[i];
            AtrResult a = atrResults[i];

            results.Add(new(
                Timestamp: s.Timestamp,
                Centerline: s.Sma,
                UpperBand: s.Sma + (multiplier * a.Atr),
                LowerBand: s.Sma - (multiplier * a.Atr)));
        }

        return results;
    }
}
