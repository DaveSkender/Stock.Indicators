namespace Skender.Stock.Indicators;

// CHAIKIN OSCILLATOR (SERIES)
public static partial class Indicator
{
    internal static List<ChaikinOscResult> CalcChaikinOsc(
        this List<QuoteD> qdList,
        int fastPeriods,
        int slowPeriods)
    {
        // check parameter arguments
        ValidateChaikinOsc(fastPeriods, slowPeriods);

        // money flow
        List<ChaikinOscResult> results = qdList.CalcAdl(null)
            .Select(r => new ChaikinOscResult(r.Date)
            {
                MoneyFlowMultiplier = r.MoneyFlowMultiplier,
                MoneyFlowVolume = r.MoneyFlowVolume,
                Adl = r.Adl
            })
            .ToList();

        // EMA of ADL
        List<(DateTime Date, double)> tpAdl = results
            .Select(x => (
                x.Date, (double)(x.Adl == null ? double.NaN : x.Adl)))
            .ToList();

        List<EmaResult> adlEmaSlow = tpAdl.CalcEma(slowPeriods);
        List<EmaResult> adlEmaFast = tpAdl.CalcEma(fastPeriods);

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
