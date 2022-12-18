using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// SCHAFF TREND CYCLE (SERIES)
public static partial class Indicator
{
    internal static Collection<StcResult> CalcStc(
        this Collection<(DateTime, double)> tpColl,
        int cyclePeriods,
        int fastPeriods,
        int slowPeriods)
    {
        // check parameter arguments
        ValidateStc(cyclePeriods, fastPeriods, slowPeriods);

        // initialize results
        int length = tpColl.Count;
        int initPeriods = Math.Min(slowPeriods - 1, length);
        Collection<StcResult> results = new();

        // add back auto-pruned results
        for (int i = 0; i < initPeriods; i++)
        {
            (DateTime date, double _) = tpColl[i];
            results.Add(new StcResult(date));
        }

        // macd component
        Collection<QuoteD> macdQuotes = new();

        List<MacdResult> macdResults = tpColl
          .CalcMacd(fastPeriods, slowPeriods, 1)
          .Remove(initPeriods);

        foreach (MacdResult x in macdResults)
        {
            macdQuotes.Add(new QuoteD
            {
                Date = x.Date,
                High = x.Macd.Null2NaN(),
                Low = x.Macd.Null2NaN(),
                Close = x.Macd.Null2NaN()
            });
        }

        // get stochastic of macd
        Collection<StochResult> stochMacd = macdQuotes
          .CalcStoch(cyclePeriods, 1, 3, 3, 2, MaType.SMA);

        // add stoch results
        for (int i = 0; i < stochMacd.Count; i++)
        {
            StochResult r = stochMacd[i];
            results.Add(new StcResult(r.Date) { Stc = r.Oscillator });
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
