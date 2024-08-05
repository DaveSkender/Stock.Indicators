namespace Skender.Stock.Indicators;

// STOCHASTIC RSI (SERIES)

public static partial class Indicator
{
    private static List<StochRsiResult> CalcStochRsi<T>(
        this List<T> source,
        int rsiPeriods,
        int stochPeriods,
        int signalPeriods,
        int smoothPeriods)
        where T : IReusable
    {
        // check parameter arguments
        StochRsi.Validate(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        // initialize results
        int length = source.Count;
        int initPeriods = Math.Min(rsiPeriods + stochPeriods - 1, length);
        List<StochRsiResult> results = new(length);

        // add back auto-pruned results
        for (int i = 0; i < initPeriods; i++)
        {
            T s = source[i];
            results.Add(new(s.Timestamp));
        }

        // get Stochastic of RSI
        List<StochResult> stoResults =
            source
            .CalcRsi(rsiPeriods)
            .Remove(Math.Min(rsiPeriods, length)) // TODO: still need to Remove() here?
            .Select(x => new QuoteD(
                Timestamp: x.Timestamp,
                Open: 0,
                High: x.Rsi.Null2NaN(),
                Low: x.Rsi.Null2NaN(),
                Close: x.Rsi.Null2NaN(),
                Volume: 0
             ))
            .ToList()
            .CalcStoch(
                stochPeriods,
                signalPeriods,
                smoothPeriods, 3, 2, MaType.SMA)
            .ToList();

        // add stoch results
        for (int i = rsiPeriods + stochPeriods - 1; i < length; i++)
        {
            StochResult r = stoResults[i - rsiPeriods];

            results.Add(new StochRsiResult(
                Timestamp: r.Timestamp,
                StochRsi: r.Oscillator,
                Signal: r.Signal));
        }

        return results;
    }
}
