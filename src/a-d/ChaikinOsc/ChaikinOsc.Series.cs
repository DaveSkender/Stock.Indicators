using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// CHAIKIN OSCILLATOR (SERIES)
public static partial class Indicator
{
    internal static Collection<ChaikinOscResult> CalcChaikinOsc(
        this Collection<QuoteD> qdList,
        int fastPeriods,
        int slowPeriods)
    {
        // check parameter arguments
        ValidateChaikinOsc(fastPeriods, slowPeriods);

        // money flow
        Collection<ChaikinOscResult> results = new();

        foreach (AdlResult r in qdList.CalcAdl(null))
        {
            results.Add(new ChaikinOscResult(r.Date)
            {
                MoneyFlowMultiplier = r.MoneyFlowMultiplier,
                MoneyFlowVolume = r.MoneyFlowVolume,
                Adl = r.Adl
            });
        }

        // EMA of ADL
        Collection<(DateTime Date, double)> tpAdl = new();

        foreach (ChaikinOscResult x in results)
        {
            tpAdl.Add(new(x.Date, (double)(x.Adl == null ? double.NaN : x.Adl)));
        }

        Collection<EmaResult> adlEmaSlow = tpAdl.CalcEma(slowPeriods);
        Collection<EmaResult> adlEmaFast = tpAdl.CalcEma(fastPeriods);

        // add Oscillator
        for (int i = slowPeriods - 1; i < results.Count; i++)
        {
            ChaikinOscResult r = results[i];

            EmaResult f = adlEmaFast[i];
            EmaResult s = adlEmaSlow[i];

            r.Oscillator = f.Ema - s.Ema;
        }

        return results;
    }

    // parameter validation
    private static void ValidateChaikinOsc(
        int fastPeriods,
        int slowPeriods)
    {
        // check parameter arguments
        if (fastPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fastPeriods), fastPeriods,
                "Fast lookback periods must be greater than 0 for Chaikin Oscillator.");
        }

        if (slowPeriods <= fastPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                "Slow lookback periods must be greater than Fast lookback period for Chaikin Oscillator.");
        }
    }
}
