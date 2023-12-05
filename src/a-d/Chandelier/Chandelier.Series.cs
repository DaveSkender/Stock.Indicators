namespace Skender.Stock.Indicators;

// CHANDELIER EXIT (SERIES)

public static partial class Indicator
{
    internal static List<ChandelierResult> CalcChandelier(
        this List<QuoteD> qdList,
        int lookbackPeriods,
        double multiplier,
        ChandelierType type)
    {
        // check parameter arguments
        Chandelier.Validate(lookbackPeriods, multiplier);

        // initialize
        int length = qdList.Count;
        List<ChandelierResult> results = new(length);
        List<AtrResult> atrResult = qdList
            .CalcAtr(lookbackPeriods)
            .ToList();

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            QuoteD q = qdList[i];

            ChandelierResult r = new() { Date = q.Date };
            results.Add(r);

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
                            QuoteD d = qdList[p];
                            if (d.High > maxHigh)
                            {
                                maxHigh = d.High;
                            }
                        }

                        r.ChandelierExit = maxHigh - (atr * multiplier);
                        break;

                    case ChandelierType.Short:

                        double minLow = double.MaxValue;
                        for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                        {
                            QuoteD d = qdList[p];
                            if (d.Low < minLow)
                            {
                                minLow = d.Low;
                            }
                        }

                        r.ChandelierExit = minLow + (atr * multiplier);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(type));
                }
            }
        }

        return results;
    }
}
