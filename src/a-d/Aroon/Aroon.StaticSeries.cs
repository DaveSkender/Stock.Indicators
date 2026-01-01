namespace Skender.Stock.Indicators;

/// <summary>
/// Aroon Oscillator indicator.
/// </summary>
public static partial class Aroon
{
    /// <summary>
    /// Calculates the Aroon Oscillator from a series of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A list of Aroon results.</returns>
    public static IReadOnlyList<AroonResult> ToAroon(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 25)
        => quotes
            .ToQuoteDList()
            .CalcAroon(lookbackPeriods);

    /// <summary>
    /// Calculates the Aroon Oscillator for the given source data.
    /// </summary>
    /// <param name="quotes">The source data.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A list of Aroon results.</returns>
    private static List<AroonResult> CalcAroon(
        this List<QuoteD> quotes,
        int lookbackPeriods)
    {
        // check parameter arguments
        Validate(lookbackPeriods);

        // initialize
        int length = quotes.Count;
        List<AroonResult> results = new(length);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = quotes[i];
            double? aroonUp = null;
            double? aroonDown = null;

            // add aroons
            if (i + 1 > lookbackPeriods)
            {
                double? lastHighPrice = 0;
                double? lastLowPrice = double.MaxValue;
                int lastHighIndex = 0;
                int lastLowIndex = 0;

                for (int p = i + 1 - lookbackPeriods - 1; p <= i; p++)
                {
                    QuoteD d = quotes[p];

                    if (d.High > lastHighPrice)
                    {
                        lastHighPrice = d.High;
                        lastHighIndex = p + 1;
                    }

                    if (d.Low < lastLowPrice)
                    {
                        lastLowPrice = d.Low;
                        lastLowIndex = p + 1;
                    }
                }

                aroonUp = 100d * (lookbackPeriods - (i + 1 - lastHighIndex)) / lookbackPeriods;
                aroonDown = 100d * (lookbackPeriods - (i + 1 - lastLowIndex)) / lookbackPeriods;
            }

            AroonResult r = new(
                Timestamp: q.Timestamp,
                AroonUp: aroonUp,
                AroonDown: aroonDown,
                Oscillator: aroonUp - aroonDown);

            results.Add(r);

        }

        return results;
    }
}
