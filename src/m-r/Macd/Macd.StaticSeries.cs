namespace Skender.Stock.Indicators;

// MOVING AVERAGE CONVERGENCE/DIVERGENCE (MACD) OSCILLATOR (SERIES)

public static partial class Macd
{
    public static IReadOnlyList<MacdResult> ToMacd<T>(
        this IReadOnlyList<T> source,
        int fastPeriods = 12,
        int slowPeriods = 26,
        int signalPeriods = 9)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(fastPeriods, slowPeriods, signalPeriods);

        // initialize
        int length = source.Count;
        List<MacdResult> results = new(length);

        double lastEmaFast = double.NaN;
        double lastEmaSlow = double.NaN;
        double lastEmaMacd = double.NaN;

        double kFast = 2d / (fastPeriods + 1);
        double kSlow = 2d / (slowPeriods + 1);
        double kMacd = 2d / (signalPeriods + 1);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            // Fast EMA
            double emaFast
                = i >= fastPeriods - 1 && results[i - 1].FastEma is null
                ? Sma.Increment(source, fastPeriods, i)
                : Ema.Increment(kFast, lastEmaFast, source[i].Value);

            // Slow EMA
            double emaSlow
                = i >= slowPeriods - 1 && results[i - 1].SlowEma is null
                ? Sma.Increment(source, slowPeriods, i)
                : Ema.Increment(kSlow, lastEmaSlow, source[i].Value);

            // MACD
            double macd = emaFast - emaSlow;

            // Signal
            double signal;

            if (i >= signalPeriods + slowPeriods - 2 && results[i - 1].Signal is null)
            {
                double sum = macd;
                for (int p = i - signalPeriods + 1; p < i; p++)
                {
                    sum += results[p].Value;
                }

                signal = sum / signalPeriods;
            }
            else
            {
                signal = Ema.Increment(kMacd, lastEmaMacd, macd);
            }

            // results
            results.Add(new MacdResult(
                Timestamp: source[i].Timestamp,
                Macd: macd.NaN2Null(),
                Signal: signal.NaN2Null(),
                Histogram: (macd - signal).NaN2Null(),
                FastEma: emaFast.NaN2Null(),
                SlowEma: emaSlow.NaN2Null()));

            lastEmaMacd = signal;
            lastEmaFast = emaFast;
            lastEmaSlow = emaSlow;
        }

        return results;
    }
}
