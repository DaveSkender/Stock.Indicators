namespace Skender.Stock.Indicators;

// CHAIKIN OSCILLATOR (SERIES)

public static partial class Indicator
{
    private static List<ChaikinOscResult> CalcChaikinOsc(
        this List<QuoteD> qdList,
        int fastPeriods,
        int slowPeriods)
    {
        // check parameter arguments
        ChaikinOsc.Validate(fastPeriods, slowPeriods);

        // initialize
        int length = qdList.Count;
        List<ChaikinOscResult> results = new(length);

        // money flow
        List<AdlResult> adlResults = qdList.CalcAdl();

        // fast/slow EMA of ADL
        List<EmaResult> adlEmaSlow = adlResults.CalcEma(slowPeriods);
        List<EmaResult> adlEmaFast = adlResults.CalcEma(fastPeriods);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            AdlResult a = adlResults[i];
            EmaResult f = adlEmaFast[i];
            EmaResult s = adlEmaSlow[i];

            results.Add(new(
                 Timestamp: a.Timestamp,
                 MoneyFlowMultiplier: a.MoneyFlowMultiplier,
                 MoneyFlowVolume: a.MoneyFlowVolume,
                 Adl: a.Adl,
                 Oscillator: f.Ema - s.Ema
             ));
        }

        return results;
    }
}
