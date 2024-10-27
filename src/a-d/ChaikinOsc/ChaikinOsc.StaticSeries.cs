namespace Skender.Stock.Indicators;

// CHAIKIN OSCILLATOR (SERIES)

public static partial class ChaikinOsc
{
    public static IReadOnlyList<ChaikinOscResult> ToChaikinOsc<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int fastPeriods = 3,
        int slowPeriods = 10)
        where TQuote : IQuote
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(quotes);
        Validate(fastPeriods, slowPeriods);

        // initialize
        int length = quotes.Count;
        List<ChaikinOscResult> results = new(length);

        // money flow
        IReadOnlyList<AdlResult> adlResults = quotes.ToAdl();

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
