namespace Skender.Stock.Indicators;

// VOLATILITY SYSTEM/STOP (SERIES)

public static partial class Indicator
{
    internal static List<VolatilityStopResult> CalcVolatilityStop(
        this List<QuoteD> qdList,
        int lookbackPeriods,
        double multiplier)
    {
        //convert quotes
        List<IReusable> reList = qdList
            .Cast<IReusable>()
            .ToList();

        // check parameter arguments
        VolatilityStop.Validate(lookbackPeriods, multiplier);

        // initialize
        int length = qdList.Count;
        List<VolatilityStopResult> results = new(length);

        if (length == 0)
        {
            return results;
        }

        List<AtrResult> atrList = qdList.CalcAtr(lookbackPeriods);

        // initial trend (guess)
        int initPeriods = Math.Min(length, lookbackPeriods);
        double sic = reList[0].Value;
        bool isLong = reList[initPeriods - 1].Value > sic;

        for (int i = 0; i < initPeriods; i++)
        {
            IReusable init = reList[i];
            sic = isLong ? Math.Max(sic, init.Value) : Math.Min(sic, init.Value);
            results.Add(new VolatilityStopResult { Timestamp = init.Timestamp });
        }

        // roll through quotes
        for (int i = lookbackPeriods; i < length; i++)
        {
            IReusable s = reList[i];

            // average true range × multiplier constant
            double? arc = atrList[i - 1].Atr * multiplier;

            // stop and reverse threshold
            double? sar = isLong ? sic - arc : sic + arc;

            // add SAR as separate bands
            double? lowerBand = null;
            double? upperBand = null;

            if (isLong)
            {
                lowerBand = sar;
            }
            else
            {
                upperBand = sar;
            }

            // evaluate stop and reverse
            bool? isStop;

            if ((isLong && s.Value < sar)
            || (!isLong && s.Value > sar))
            {
                isStop = true;
                sic = s.Value;
                isLong = !isLong;
            }
            else
            {
                isStop = false;

                // significant close adjustment
                // extreme favorable close (value) while in trade
                sic = isLong ? Math.Max(sic, s.Value) : Math.Min(sic, s.Value);
            }

            results.Add(new VolatilityStopResult(
                Timestamp: s.Timestamp,
                Sar: sar,
                IsStop: isStop,
                UpperBand: upperBand,
                LowerBand: lowerBand));
        }

        // remove first trend to stop, since it is a guess
        int cutIndex = results.FindIndex(x => x.IsStop is true);

        for (int d = 0; d <= cutIndex; d++)
        {
            VolatilityStopResult r = results[d];

            results[d] = r with {
                Sar = null,
                UpperBand = null,
                LowerBand = null,
                IsStop = null
            };
        }

        return results;
    }
}
