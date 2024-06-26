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
        List<ChaikinOscResult> results = qdList.CalcAdl()
            .Select(r => new ChaikinOscResult {
                Timestamp = r.Timestamp,
                MoneyFlowMultiplier = r.MoneyFlowMultiplier,
                MoneyFlowVolume = r.MoneyFlowVolume,
                Adl = r.Adl
            })
            .ToList();

        // EMA of ADL
        List<EmaResult> adlEmaSlow = results.CalcEma(slowPeriods);
        List<EmaResult> adlEmaFast = results.CalcEma(fastPeriods);

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
