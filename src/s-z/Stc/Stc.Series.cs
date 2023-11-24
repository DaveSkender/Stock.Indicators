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
        List<StcResult> results = new(length);

        // get stochastic of macd
        List<StochResult> stochMacd = tpList
          .CalcMacd(fastPeriods, slowPeriods, 1)
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
        for (int i = 0; i < length; i++)
        {
            StochResult r = stochMacd[i];
            results.Add(new StcResult(r.Date) { Stc = r.Oscillator });
        }

        return results;
    }
}
