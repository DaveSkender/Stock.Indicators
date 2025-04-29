namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Aroon Oscillator.
/// </summary>
public static partial class Aroon
{
    /// <summary>
    /// Calculates the Aroon Oscillator from a series of quotes.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
    /// <param name="quotes">The list of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods to look back. Default is 25.</param>
    /// <returns>A list of Aroon results.</returns>
    [Series("AROON", "Aroon Up/Down", Category.PriceTrend, ChartType.Oscillator, "AROON([P1]) Up/Down")]
    public static IReadOnlyList<AroonResult> ToAroon<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        [ParamNum<int>("Lookback Periods", 1, 250, 25)]
        int lookbackPeriods = 25)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcAroon(lookbackPeriods);

    /// <summary>
    /// Calculates the Aroon Oscillator for the given source data.
    /// </summary>
    /// <param name="source">The source data.</param>
    /// <param name="lookbackPeriods">The number of periods to look back.</param>
    /// <returns>A list of Aroon results.</returns>
    private static List<AroonResult> CalcAroon(
        this List<QuoteD> source,
        int lookbackPeriods)
    {
        // check parameter arguments
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<AroonResult> results = new(length);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = source[i];
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
                    QuoteD d = source[p];

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
