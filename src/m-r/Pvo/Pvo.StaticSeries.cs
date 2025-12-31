namespace Skender.Stock.Indicators;

/// <summary>
/// Percentage Volume Oscillator (PVO) series indicator.
/// </summary>
public static partial class Pvo
{
    /// <summary>
    /// Converts a list of quotes to a list of PVO results.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="fastPeriods">The number of periods for the fast EMA.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <returns>A list of PVO results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quotes list is null.</exception>
    public static IReadOnlyList<PvoResult> ToPvo(
        this IReadOnlyList<IQuote> quotes,
        int fastPeriods = 12,
        int slowPeriods = 26,
        int signalPeriods = 9)
        => quotes
            .Use(CandlePart.Volume)
            .CalcPvo(fastPeriods, slowPeriods, signalPeriods);

    /// <summary>
    /// Calculates the PVO values.
    /// </summary>
    /// <param name="source">The list of volume values.</param>
    /// <param name="fastPeriods">The number of periods for the fast EMA.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <returns>A list of PVO results.</returns>
    private static List<PvoResult> CalcPvo(
        this IReadOnlyList<IReusable> source,  // volume
        int fastPeriods,
        int slowPeriods,
        int signalPeriods)

    {
        // check parameter arguments
        Validate(fastPeriods, slowPeriods, signalPeriods);

        // initialize
        int length = source.Count;
        List<PvoResult> results = new(length);

        double lastEmaFast = double.NaN;
        double lastEmaSlow = double.NaN;
        double lastEmaPvo = double.NaN;

        double kFast = 2d / (fastPeriods + 1);
        double kSlow = 2d / (slowPeriods + 1);
        double kPvo = 2d / (signalPeriods + 1);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            IReusable s = source[i];

            // re-initialize Fast EMA
            double emaFast;

            if (double.IsNaN(lastEmaFast) && i >= fastPeriods - 1)
            {
                double sum = 0;
                for (int p = i - fastPeriods + 1; p <= i; p++)
                {
                    IReusable ps = source[p];
                    sum += ps.Value;
                }

                emaFast = sum / fastPeriods;
            }
            else
            {
                emaFast = Ema.Increment(kFast, lastEmaFast, s.Value);
            }

            // re-initialize Slow EMA
            double emaSlow;

            if (double.IsNaN(lastEmaSlow) && i >= slowPeriods - 1)
            {
                double sum = 0;
                for (int p = i - slowPeriods + 1; p <= i; p++)
                {
                    IReusable ps = source[p];
                    sum += ps.Value;
                }

                emaSlow = sum / slowPeriods;
            }
            else
            {
                emaSlow = Ema.Increment(kSlow, lastEmaSlow, s.Value);
            }

            double pvo = emaSlow != 0 ?
                100 * ((emaFast - emaSlow) / emaSlow) : double.NaN;

            // re-initialize Signal EMA
            double signal;

            if (double.IsNaN(lastEmaPvo) && i >= signalPeriods + slowPeriods - 2)
            {
                double sum = pvo;
                for (int p = i - signalPeriods + 1; p < i; p++)
                {
                    sum += results[p].Value;
                }

                signal = sum / signalPeriods;
            }
            else
            {
                signal = Ema.Increment(kPvo, lastEmaPvo, pvo);
            }

            // write results
            results.Add(new(
                Timestamp: s.Timestamp,
                Pvo: pvo.NaN2Null(),
                Signal: signal.NaN2Null(),
                Histogram: (pvo - signal).NaN2Null()));

            lastEmaPvo = signal;
            lastEmaFast = emaFast;
            lastEmaSlow = emaSlow;
        }

        return results;
    }

    /* DESIGN NOTE: this is exactly like MACD, except for:
     *   a) it uses Volume instead of Price (see API)
     *   b) the PVO calculation slightly different     */
}
