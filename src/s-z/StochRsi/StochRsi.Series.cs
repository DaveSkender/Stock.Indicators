using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// STOCHASTIC RSI (SERIES)
public static partial class Indicator
{
    internal static Collection<StochRsiResult> CalcStochRsi(
        this Collection<(DateTime, double)> tpColl,
        int rsiPeriods,
        int stochPeriods,
        int signalPeriods,
        int smoothPeriods)
    {
        // check parameter arguments
        ValidateStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        // initialize results
        int length = tpColl.Count;
        int initPeriods = Math.Min(rsiPeriods + stochPeriods - 1, length);
        Collection<StochRsiResult> results = new();

        // add back auto-pruned results
        for (int i = 0; i < initPeriods; i++)
        {
            (DateTime date, double _) = tpColl[i];
            results.Add(new StochRsiResult(date));
        }

        // get RSI component
        Collection<QuoteD> rsiQuotes = new();

        List<RsiResult> rsiResults = tpColl
            .CalcRsi(rsiPeriods)
            .Remove(Math.Min(rsiPeriods, length));

        foreach (RsiResult x in rsiResults)
        {
            rsiQuotes.Add(new QuoteD
            {
                Date = x.Date,
                High = x.Rsi.Null2NaN(),
                Low = x.Rsi.Null2NaN(),
                Close = x.Rsi.Null2NaN()
            });
        }

        // get Stochastic of RSI
        Collection<StochResult> stoResults = rsiQuotes
            .CalcStoch(
                stochPeriods,
                signalPeriods,
                smoothPeriods, 3, 2, MaType.SMA);

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
