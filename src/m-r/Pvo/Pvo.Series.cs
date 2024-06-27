namespace Skender.Stock.Indicators;

// PRICE VOLUME OSCILLATOR (SERIES)

public static partial class Indicator
{
    internal static List<PvoResult> CalcPvo<T>(
        this List<T> source,
        int fastPeriods,
        int slowPeriods,
        int signalPeriods)
        where T : IReusable
    {
        // check parameter arguments
        Pvo.Validate(fastPeriods, slowPeriods, signalPeriods);

        // initialize
        List<EmaResult> emaFast = source.CalcEma(fastPeriods);
        List<EmaResult> emaSlow = source.CalcEma(slowPeriods);

        int length = source.Count;
        List<Reusable> emaDiff = [];
        List<PvoResult> results = new(length);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            var s = source[i];
            EmaResult df = emaFast[i];
            EmaResult ds = emaSlow[i];

            PvoResult r = new() { Timestamp = s.Timestamp };
            results.Add(r);

            if (i >= slowPeriods - 1)
            {
                double? pvo = (ds.Ema != 0) ?
                    100 * ((df.Ema - ds.Ema) / ds.Ema) : null;

                r.Pvo = pvo;

                // temp data for interim EMA of PVO
                var diff = new Reusable(s.Timestamp, (pvo == null) ? 0 : (double)pvo);

                emaDiff.Add(diff);
            }
        }

        // add signal and histogram to result
        List<EmaResult> emaSignal = CalcEma(emaDiff, signalPeriods);

        for (int d = slowPeriods - 1; d < length; d++)
        {
            PvoResult r = results[d];
            EmaResult ds = emaSignal[d + 1 - slowPeriods];

            r.Signal = ds.Ema;
            r.Histogram = r.Pvo - r.Signal;
        }

        return results;
    }
}
