namespace Skender.Stock.Indicators;

// SCHAFF TREND CYCLE (SERIES)

public static partial class Indicator
{
    private static List<StcResult> CalcStc<T>(
        this List<T> source,
        int cyclePeriods,
        int fastPeriods,
        int slowPeriods)
        where T : IReusable
    {
        // check parameter arguments
        Stc.Validate(cyclePeriods, fastPeriods, slowPeriods);

        // initialize results
        int length = source.Count;
        List<StcResult> results = new(length);

        // get stochastic of macd
        IReadOnlyList<StochResult> stochMacd = source
          .ToMacd(fastPeriods, slowPeriods, 1)
          .Select(x => new QuoteD(
              x.Timestamp, 0,
              x.Macd.Null2NaN(),
              x.Macd.Null2NaN(),
              x.Macd.Null2NaN(), 0))
          .ToList()
          .CalcStoch(cyclePeriods, 1, 3, 3, 2, MaType.SMA);

        // add stoch results
        for (int i = 0; i < length; i++)
        {
            StochResult r = stochMacd[i];

            results.Add(new StcResult(
                Timestamp: r.Timestamp,
                Stc: r.Oscillator));
        }

        return results;
    }
}
