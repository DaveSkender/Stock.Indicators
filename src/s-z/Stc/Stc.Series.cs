namespace Skender.Stock.Indicators;

// SCHAFF TREND CYCLE (SERIES)

public static partial class Indicator
{
    internal static List<StcResult> CalcStc(
        this List<(DateTime, double)> tpList,
        int cyclePeriods,
        int fastPeriods,
        int slowPeriods)
    {
        // check parameter arguments
        Stc.Validate(cyclePeriods, fastPeriods, slowPeriods);

        // initialize results
        int length = tpList.Count;
        int initPeriods = Math.Min(slowPeriods - 1, length);
        List<StcResult> results = new(length);

        // add back auto-pruned results
        for (int i = 0; i < initPeriods; i++)
        {
            (DateTime date, double _) = tpList[i];
            results.Add(new StcResult(date));
        }

        // get stochastic of macd
        List<StochResult> stochMacd = tpList
          .CalcMacd(fastPeriods, slowPeriods, 1)
          .Remove(initPeriods)
          .Select(x => new QuoteD
          {
              Date = x.Date,
              High = x.Macd.Null2NaN(),
              Low = x.Macd.Null2NaN(),
              Close = x.Macd.Null2NaN()
          })
          .ToList()
          .CalcStoch(cyclePeriods, 1, 3, 3, 2, MaType.SMA);

        // add stoch results
        for (int i = 0; i < stochMacd.Count; i++)
        {
            StochResult r = stochMacd[i];
            results.Add(new StcResult(r.Date) { Stc = r.Oscillator });
        }

        return results;
    }
}
