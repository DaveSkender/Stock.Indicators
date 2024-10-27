namespace Skender.Stock.Indicators;

// SCHAFF TREND CYCLE (SERIES)

public static partial class Stc
{
    public static IReadOnlyList<StcResult> ToStc<T>(
        this IReadOnlyList<T> source,
        int cyclePeriods = 10,
        int fastPeriods = 23,
        int slowPeriods = 50)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(cyclePeriods, fastPeriods, slowPeriods);

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
