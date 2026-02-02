namespace Skender.Stock.Indicators;

/// <summary>
/// Chaikin Oscillator on a series of quotes indicator.
/// </summary>
public static partial class ChaikinOsc
{
    /// <summary>
    /// Calculates the Chaikin Oscillator for a series of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="fastPeriods">The number of periods to use for the fast EMA. Default is 3.</param>
    /// <param name="slowPeriods">The number of periods to use for the slow EMA. Default is 10.</param>
    /// <returns>A read-only list of <see cref="ChaikinOscResult"/> containing the Chaikin Oscillator calculation results.</returns>
    public static IReadOnlyList<ChaikinOscResult> ToChaikinOsc(
        this IReadOnlyList<IQuote> quotes,
        int fastPeriods = 3,
        int slowPeriods = 10)
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
                 Oscillator: f.Ema - s.Ema,
                 FastEma: f.Ema,
                 SlowEma: s.Ema
             ));
        }

        return results;
    }
}
