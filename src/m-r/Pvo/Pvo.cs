namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // PRICE VOLUME OSCILLATOR (PVO)
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<PvoResult> GetPvo<TQuote>(
        this IEnumerable<TQuote> quotes,
        int fastPeriods = 12,
        int slowPeriods = 26,
        int signalPeriods = 9)
        where TQuote : IQuote
    {
        // convert quotes
        List<(DateTime Date, double Value)> tpList = quotes.ToBasicTuple(CandlePart.Volume);

        // check parameter arguments
        ValidatePvo(fastPeriods, slowPeriods, signalPeriods);

        // initialize
        List<EmaResult> emaFast = CalcEma(tpList, fastPeriods);
        List<EmaResult> emaSlow = CalcEma(tpList, slowPeriods);

        int length = tpList.Count;
        List<(DateTime, double)> emaDiff = new();
        List<PvoResult> results = new(length);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];
            EmaResult df = emaFast[i];
            EmaResult ds = emaSlow[i];

            PvoResult result = new()
            {
                Date = date
            };

            if (i >= slowPeriods - 1)
            {
                double? pvo = (ds.Ema != 0) ?
                    100 * (double?)((df.Ema - ds.Ema) / ds.Ema) : null;

                result.Pvo = pvo;

                // temp data for interim EMA of PVO
                (DateTime, double) diff = (date, (pvo == null) ? 0 : (double)pvo);

                emaDiff.Add(diff);
            }

            results.Add(result);
        }

        // add signal and histogram to result
        List<EmaResult> emaSignal = CalcEma(emaDiff, signalPeriods);

        for (int d = slowPeriods - 1; d < length; d++)
        {
            PvoResult r = results[d];
            EmaResult ds = emaSignal[d + 1 - slowPeriods];

            r.Signal = (double?)ds.Ema;
            r.Histogram = r.Pvo - r.Signal;
        }

        return results;
    }

    // parameter validation
    private static void ValidatePvo(
        int fastPeriods,
        int slowPeriods,
        int signalPeriods)
    {
        // check parameter arguments
        if (fastPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fastPeriods), fastPeriods,
                "Fast periods must be greater than 0 for PVO.");
        }

        if (signalPeriods < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                "Signal periods must be greater than or equal to 0 for PVO.");
        }

        if (slowPeriods <= fastPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                "Slow periods must be greater than the fast period for PVO.");
        }
    }
}
