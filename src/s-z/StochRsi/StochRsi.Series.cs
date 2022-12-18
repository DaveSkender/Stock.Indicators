namespace Skender.Stock.Indicators;

// STOCHASTIC RSI (SERIES)
public static partial class Indicator
{
    internal static List<StochRsiResult> CalcStochRsi(
        this List<(DateTime, double)> tpList,
        int rsiPeriods,
        int stochPeriods,
        int signalPeriods,
        int smoothPeriods)
    {
        // check parameter arguments
        ValidateStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        // initialize results
        int length = tpList.Count;
        int initPeriods = Math.Min(rsiPeriods + stochPeriods - 1, length);
        List<StochRsiResult> results = new(length);

        // add back auto-pruned results
        for (int i = 0; i < initPeriods; i++)
        {
            (DateTime date, double _) = tpList[i];
            results.Add(new StochRsiResult(date));
        }

        // get Stochastic of RSI
        List<StochResult> stoResults =
            tpList
            .CalcRsi(rsiPeriods)
            .Remove(Math.Min(rsiPeriods, length))
            .Select(x => new QuoteD
            {
                Date = x.Date,
                High = x.Rsi.Null2NaN(),
                Low = x.Rsi.Null2NaN(),
                Close = x.Rsi.Null2NaN()
            })
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
            results.Add(new StochRsiResult(r.Date)
            {
                StochRsi = r.Oscillator,
                Signal = r.Signal
            });
        }

        return results;
    }

    // parameter validation
    private static void ValidateStochRsi(
        int rsiPeriods,
        int stochPeriods,
        int signalPeriods,
        int smoothPeriods)
    {
        // check parameter arguments
        if (rsiPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rsiPeriods), rsiPeriods,
                "RSI periods must be greater than 0 for Stochastic RSI.");
        }

        if (stochPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(stochPeriods), stochPeriods,
                "STOCH periods must be greater than 0 for Stochastic RSI.");
        }

        if (signalPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                "Signal periods must be greater than 0 for Stochastic RSI.");
        }

        if (smoothPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smoothPeriods), smoothPeriods,
                "Smooth periods must be greater than 0 for Stochastic RSI.");
        }
    }
}
