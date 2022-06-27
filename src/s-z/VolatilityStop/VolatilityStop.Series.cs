namespace Skender.Stock.Indicators;

// VOLATILITY SYSTEM/STOP (SERIES)
public static partial class Indicator
{
    internal static List<VolatilityStopResult> CalcVolatilityStop(
        this List<QuoteD> qdList,
        int lookbackPeriods,
        double multiplier)
    {
        // convert quotes
        List<(DateTime, double)> tpList = qdList
            .ToBasicTuple(CandlePart.Close);

        // check parameter arguments
        ValidateVolatilityStop(lookbackPeriods, multiplier);

        // initialize
        int length = tpList.Count;
        List<VolatilityStopResult> results = new(length);

        if (length == 0)
        {
            return results;
        }

        List<AtrResult> atrList = qdList.CalcAtr(lookbackPeriods);

        // initial trend (guess)
        int initPeriods = Math.Min(length, lookbackPeriods);
        double sic = tpList[0].Item2;
        bool isLong = tpList[initPeriods - 1].Item2 > sic;

        for (int i = 0; i < initPeriods; i++)
        {
            (DateTime date, double value) = tpList[i];
            sic = isLong ? Math.Max(sic, value) : Math.Min(sic, value);
            results.Add(new VolatilityStopResult(date));
        }

        // roll through quotes
        for (int i = lookbackPeriods; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];

            // average true range × multiplier constant
            double? arc = atrList[i - 1].Atr * multiplier;

            VolatilityStopResult r = new(date)
            {
                // stop and reverse threshold
                Sar = isLong ? sic - arc : sic + arc
            };
            results.Add(r);

            // add SAR as separate bands
            if (isLong)
            {
                r.LowerBand = r.Sar;
            }
            else
            {
                r.UpperBand = r.Sar;
            }

            // evaluate stop and reverse
            if ((isLong && value < r.Sar)
            || (!isLong && value > r.Sar))
            {
                r.IsStop = true;
                sic = value;
                isLong = !isLong;
            }
            else
            {
                r.IsStop = false;

                // significant close adjustment
                // extreme favorable close (value) while in trade
                sic = isLong ? Math.Max(sic, value) : Math.Min(sic, value);
            }
        }

        // remove first trend to stop, since it is a guess
        VolatilityStopResult? firstStop = results
            .Where(x => x.IsStop == true)
            .OrderBy(x => x.Date)
            .FirstOrDefault();

        if (firstStop != null)
        {
            int cutIndex = results.IndexOf(firstStop);

            for (int d = 0; d <= cutIndex; d++)
            {
                VolatilityStopResult r = results[d];
                r.Sar = null;
                r.UpperBand = null;
                r.LowerBand = null;
                r.IsStop = null;
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateVolatilityStop(
        int lookbackPeriods,
        double multiplier)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for Volatility Stop.");
        }

        if (multiplier <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(multiplier), multiplier,
                "ATR Multiplier must be greater than 0 for Volatility Stop.");
        }
    }
}
