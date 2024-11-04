namespace Skender.Stock.Indicators;

/// <summary>
/// Provides extension methods for calculating the Chandelier Exit on a series of quotes.
/// </summary>
public static partial class Chandelier
{
    /// <summary>
    /// Calculates the Chandelier Exit for a series of quotes.
    /// </summary>
    /// <typeparam name="TQuote">The type of the elements in the quotes list, which must implement <see cref="IQuote"/>.</typeparam>
    /// <param name="quotes">The source list of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods to use for the lookback window. Default is 22.</param>
    /// <param name="multiplier">The multiplier to apply to the ATR. Default is 3.</param>
    /// <param name="type">The type of Chandelier Exit to calculate (Long or Short). Default is Long.</param>
    /// <returns>A read-only list of <see cref="ChandelierResult"/> containing the Chandelier Exit calculation results.</returns>
    public static IReadOnlyList<ChandelierResult> ToChandelier<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 22,
        double multiplier = 3,
        ChandelierType type = ChandelierType.Long)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcChandelier(lookbackPeriods, multiplier, type);

    /// <summary>
    /// Calculates the Chandelier Exit for a series of quotes.
    /// </summary>
    /// <param name="source">The source list of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods to use for the lookback window.</param>
    /// <param name="multiplier">The multiplier to apply to the ATR.</param>
    /// <param name="type">The type of Chandelier Exit to calculate (Long or Short).</param>
    /// <returns>A list of <see cref="ChandelierResult"/> containing the Chandelier Exit calculation results.</returns>
    private static List<ChandelierResult> CalcChandelier(
        this IReadOnlyList<QuoteD> source,
        int lookbackPeriods,
        double multiplier,
        ChandelierType type)
    {
        // check parameter arguments
        Validate(lookbackPeriods, multiplier);

        // initialize
        int length = source.Count;
        List<ChandelierResult> results = new(length);

        IReadOnlyList<AtrResult> atrResult
            = source.CalcAtr(lookbackPeriods);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = source[i];

            double? exit = null;

            // add exit values
            if (i >= lookbackPeriods)
            {
                double? atr = atrResult[i].Atr;

                switch (type)
                {
                    case ChandelierType.Long:

                        double maxHigh = 0;
                        for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                        {
                            QuoteD d = source[p];
                            if (d.High > maxHigh)
                            {
                                maxHigh = d.High;
                            }
                        }

                        exit = maxHigh - (atr * multiplier);
                        break;

                    case ChandelierType.Short:

                        double minLow = double.MaxValue;
                        for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                        {
                            QuoteD d = source[p];
                            if (d.Low < minLow)
                            {
                                minLow = d.Low;
                            }
                        }

                        exit = minLow + (atr * multiplier);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(type));
                }
            }

            results.Add(new(
                Timestamp: q.Timestamp,
                ChandelierExit: exit));
        }

        return results;
    }
}
