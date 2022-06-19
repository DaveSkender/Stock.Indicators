namespace Skender.Stock.Indicators;
#nullable disable // false positive in QuoteD conversion

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
        ValidateStc(cyclePeriods, fastPeriods, slowPeriods);

        // initialize results
        int length = tpList.Count;
        int initPeriods = Math.Min(slowPeriods - 1, length);
        List<StcResult> results = new(length);

        // add back auto-pruned results
        for (int i = 0; i < initPeriods; i++)
        {
            (DateTime date, double _) = tpList[i];
            results.Add(new StcResult() { Date = date });
        }

        // get stochastic of macd
        List<StochResult> stochMacd = tpList
          .CalcMacd(fastPeriods, slowPeriods, 1)
          .Remove(slowPeriods - 1)
          .Where(x => x.Macd is not null)
          .Select(x => new QuoteD
          {
              Date = x.Date,
              High = (double)x.Macd,
              Low = (double)x.Macd,
              Close = (double)x.Macd
          })
          .ToList()
          .CalcStoch(cyclePeriods, 1, 3, 3, 2, MaType.SMA);

        // add stoch results
        for (int i = 0; i < stochMacd.Count; i++)
        {
            StochResult r = stochMacd[i];
            results.Add(new StcResult { Date = r.Date, Stc = r.Oscillator });
        }

        return results;
    }

    // parameter validation
    private static void ValidateStc(
        int cyclePeriods,
        int fastPeriods,
        int slowPeriods)
    {
        // check parameter arguments
        if (cyclePeriods < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cyclePeriods), cyclePeriods,
                "Trend Cycle periods must be greater than or equal to 0 for STC.");
        }

        if (fastPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fastPeriods), fastPeriods,
                "Fast periods must be greater than 0 for STC.");
        }

        if (slowPeriods <= fastPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                "Slow periods must be greater than the fast period for STC.");
        }
    }
}
