namespace Skender.Stock.Indicators;

// CHAIKIN OSCILLATOR (SERIES)

public static partial class Indicator
{
    private static List<ChaikinOscResult> CalcChaikinOsc<TQuote>(
        this List<TQuote> source,
        int fastPeriods,
        int slowPeriods)
        where TQuote : IQuote
    {
        // check parameter arguments
        ChaikinOsc.Validate(fastPeriods, slowPeriods);

        // initialize
        int length = source.Count;
        List<ChaikinOscResult> results = new(length);

        // money flow
        var adlResults = source.ToAdl();

        // fast/slow EMA of ADL
        IReadOnlyList<EmaResult> adlEmaSlow = adlResults.ToEma(slowPeriods);
        IReadOnlyList<EmaResult> adlEmaFast = adlResults.ToEma(fastPeriods);

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
