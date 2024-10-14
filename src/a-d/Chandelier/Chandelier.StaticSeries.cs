namespace Skender.Stock.Indicators;

// CHANDELIER EXIT (SERIES)

public static partial class Chandelier
{
    public static IReadOnlyList<ChandelierResult> ToChandelier<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 22,
        double multiplier = 3,
        ChandelierType type = ChandelierType.Long)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcChandelier(lookbackPeriods, multiplier, type);

    private static List<ChandelierResult> CalcChandelier(
        this List<QuoteD> source,
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
