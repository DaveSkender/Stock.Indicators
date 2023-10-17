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
        ChaikinOsc.Validate(fastPeriods, slowPeriods);

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
                x.Date, (double)(x.Adl ?? double.NaN)))
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
}
