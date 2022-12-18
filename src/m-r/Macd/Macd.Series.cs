using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// MOVING AVERAGE CONVERGENCE/DIVERGENCE (MACD) OSCILLATOR (SERIES)
public static partial class Indicator
{
    internal static Collection<MacdResult> CalcMacd(
        this Collection<(DateTime, double)> tpColl,
        int fastPeriods,
        int slowPeriods,
        int signalPeriods)
    {
        // check parameter arguments
        ValidateMacd(fastPeriods, slowPeriods, signalPeriods);

        // initialize
        Collection<EmaResult> emaFast = tpColl.CalcEma(fastPeriods);
        Collection<EmaResult> emaSlow = tpColl.CalcEma(slowPeriods);

        int length = tpColl.Count;
        Collection<(DateTime, double)> emaDiff = new();
        Collection<MacdResult> results = new();

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double _) = tpColl[i];
            EmaResult df = emaFast[i];
            EmaResult ds = emaSlow[i];

            MacdResult r = new(date)
            {
                FastEma = df.Ema,
                SlowEma = ds.Ema
            };
            results.Add(r);

            if (i >= slowPeriods - 1)
            {
                double macd = (df.Ema - ds.Ema).Null2NaN();
                r.Macd = macd.NaN2Null();

                // temp data for interim EMA of macd
                (DateTime, double) diff = (date, macd);

                emaDiff.Add(diff);
            }
        }

        // add signal and histogram to result
        Collection<EmaResult> emaSignal = CalcEma(emaDiff, signalPeriods);

        for (int d = slowPeriods - 1; d < length; d++)
        {
            MacdResult r = results[d];
            EmaResult ds = emaSignal[d + 1 - slowPeriods];

            r.Signal = ds.Ema.NaN2Null();
            r.Histogram = (r.Macd - r.Signal).NaN2Null();
        }

        return results;
    }

    // parameter validation
    private static void ValidateMacd(
        int fastPeriods,
        int slowPeriods,
        int signalPeriods)
    {
        // check parameter arguments
        if (fastPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fastPeriods), fastPeriods,
                "Fast periods must be greater than 0 for MACD.");
        }

        if (signalPeriods < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                "Signal periods must be greater than or equal to 0 for MACD.");
        }

        if (slowPeriods <= fastPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                "Slow periods must be greater than the fast period for MACD.");
        }
    }
}
