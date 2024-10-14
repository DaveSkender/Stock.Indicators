namespace Skender.Stock.Indicators;

// ELDER-RAY (SERIES)

public static partial class ElderRay
{
    public static IReadOnlyList<ElderRayResult> ToElderRay<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 13)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcElderRay(lookbackPeriods);

    private static List<ElderRayResult> CalcElderRay(
        this List<QuoteD> source,
        int lookbackPeriods)
    {
        // check parameter arguments
        Validate(lookbackPeriods);

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
