namespace Skender.Stock.Indicators;

/// <summary>
/// Chandelier Exit on a series of quotes indicator.
/// </summary>
public static partial class Chandelier
{
    /// <summary>
    /// Calculates the Chandelier Exit for a series of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">The multiplier to apply to the ATR.</param>
    /// <param name="type">The type of Chandelier Exit to calculate (Long or Short).</param>
    /// <returns>A read-only list of <see cref="ChandelierResult"/> containing the Chandelier Exit calculation results.</returns>
    public static IReadOnlyList<ChandelierResult> ToChandelier(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 22,
        double multiplier = 3,
        Direction type = Direction.Long)
        => quotes
            .ToQuoteDList()
            .CalcChandelier(lookbackPeriods, multiplier, type);

    /// <summary>
    /// Calculates the Chandelier Exit for a series of quotes.
    /// </summary>
    /// <param name="quotes">The source list of quotes.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">The multiplier to apply to the ATR.</param>
    /// <param name="type">The type of Chandelier Exit to calculate (Long or Short).</param>
    /// <returns>A list of <see cref="ChandelierResult"/> containing the Chandelier Exit calculation results.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a parameter is out of the valid range</exception>
    private static List<ChandelierResult> CalcChandelier(
        this List<QuoteD> quotes,
        int lookbackPeriods,
        double multiplier,
        Direction type)
    {
        // check parameter arguments
        Validate(lookbackPeriods, multiplier);

        // initialize
        int length = quotes.Count;
        List<ChandelierResult> results = new(length);

        List<AtrResult> atrResult
            = quotes.CalcAtr(lookbackPeriods);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = quotes[i];

            double? exit = null;

            // add exit values
            if (i >= lookbackPeriods)
            {
                double? atr = atrResult[i].Atr;

                switch (type)
                {
                    case Direction.Long:

                        double maxHigh = 0;
                        for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                        {
                            QuoteD d = quotes[p];
                            if (d.High > maxHigh)
                            {
                                maxHigh = d.High;
                            }
                        }

                        exit = maxHigh - (atr * multiplier);
                        break;

                    case Direction.Short:

                        double minLow = double.MaxValue;
                        for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                        {
                            QuoteD d = quotes[p];
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
