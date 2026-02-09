namespace Skender.Stock.Indicators;

/// <summary>
/// Elder Ray indicator.
/// </summary>
public static partial class ElderRay
{
    /// <summary>
    /// Converts a list of quotes to Elder Ray results.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A list of Elder Ray results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quotes list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="lookbackPeriods"/> is invalid.</exception>
    public static IReadOnlyList<ElderRayResult> ToElderRay(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 13)
        => quotes
            .ToQuoteDList()
            .CalcElderRay(lookbackPeriods);

    /// <summary>
    /// Calculates the Elder Ray indicator.
    /// </summary>
    /// <param name="quotes">The source list of quotes.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A list of Elder Ray results.</returns>
    private static List<ElderRayResult> CalcElderRay(
        this List<QuoteD> quotes,
        int lookbackPeriods)
    {
        // check parameter arguments
        Validate(lookbackPeriods);

        // initialize
        int length = quotes.Count;
        List<ElderRayResult> results = new(length);

        // EMA
        IReadOnlyList<EmaResult> emaResults
            = quotes.ToEma(lookbackPeriods);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = quotes[i];
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
