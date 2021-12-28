namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // STOCHASTIC OSCILLATOR
    /// <include file='./info.xml' path='indicator/type[@name="Main"]/*' />
    ///
    public static IEnumerable<StochResult> GetStoch<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 14,
        int signalPeriods = 3,
        int smoothPeriods = 3)
        where TQuote : IQuote
    {
        return quotes
            .GetStoch(
                lookbackPeriods,
                signalPeriods,
                smoothPeriods, 3, 2, MaType.SMA);
    }

    /// <include file='./info.xml' path='indicator/type[@name="Extended"]/*' />
    ///
    public static IEnumerable<StochResult> GetStoch<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        int signalPeriods,
        int smoothPeriods,
        decimal kFactor,
        decimal dFactor,
        MaType movingAverageType)
        where TQuote : IQuote
    {
        // convert quotes
        List<QuoteD> quotesList = quotes.ConvertToList();

        // check parameter arguments
        ValidateStoch(
            quotes, lookbackPeriods, signalPeriods, smoothPeriods,
            kFactor, dFactor, movingAverageType);

        // initialize
        int size = quotesList.Count;
        List<StochResult> results = new(size);

        // roll through quotes
        for (int i = 0; i < quotesList.Count; i++)
        {
            QuoteD q = quotesList[i];
            int index = i + 1;

            StochResult result = new()
            {
                Date = q.Date
            };

            if (index >= lookbackPeriods)
            {
                double? highHigh = double.MinValue;
                double? lowLow = double.MaxValue;

                for (int p = index - lookbackPeriods; p < index; p++)
                {
                    QuoteD x = quotesList[p];

                    if (x.High > highHigh)
                    {
                        highHigh = x.High;
                    }

                    if (x.Low < lowLow)
                    {
                        lowLow = x.Low;
                    }
                }

                result.Oscillator = lowLow != highHigh
                    ? 100 * (decimal?)((q.Close - lowLow) / (highHigh - lowLow))
                    : 0;
            }

            results.Add(result);
        }

        // smooth the oscillator
        if (smoothPeriods > 1)
        {
            results = SmoothOscillator(
                results, size, lookbackPeriods, smoothPeriods, movingAverageType);
        }

        // signal (%D) and %J
        int signalIndex = lookbackPeriods + smoothPeriods + signalPeriods - 2;
        double? s = (double?)results[lookbackPeriods - 1].Oscillator;

        for (int i = lookbackPeriods - 1; i < size; i++)
        {
            StochResult r = results[i];
            int index = i + 1;

            // add signal

            if (signalPeriods <= 1)
            {
                r.Signal = r.Oscillator;
            }

            // SMA case
            else if (index >= signalIndex && movingAverageType is MaType.SMA)
            {
                double? sumOsc = 0;
                for (int p = index - signalPeriods; p < index; p++)
                {
                    StochResult x = results[p];
                    sumOsc += (double?)x.Oscillator;
                }

                r.Signal = (decimal?)(sumOsc / signalPeriods);
            }

            // SMMA case
            else if (i >= lookbackPeriods - 1 && movingAverageType is MaType.SMMA)
            {
                s = (s == null) ? (double?)results[i].Oscillator : s; // reset if null

                s = ((s * (signalPeriods - 1)) + (double?)results[i].Oscillator) / signalPeriods;
                r.Signal = (decimal?)s;
            }

            // %J
            r.PercentJ = (kFactor * r.Oscillator) - (dFactor * r.Signal);
        }

        return results;
    }

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<StochResult> RemoveWarmupPeriods(
        this IEnumerable<StochResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Oscillator != null);

        return results.Remove(removePeriods);
    }

    // internals
    private static List<StochResult> SmoothOscillator(
        List<StochResult> results,
        int size,
        int lookbackPeriods,
        int smoothPeriods,
        MaType movingAverageType)
    {
        // temporarily store interim smoothed oscillator
        double?[] smooth = new double?[size]; // smoothed value

        if (movingAverageType is MaType.SMA)
        {
            int smoothIndex = lookbackPeriods + smoothPeriods - 2;

            for (int i = smoothIndex; i < size; i++)
            {
                int index = i + 1;

                double? sumOsc = 0;
                for (int p = index - smoothPeriods; p < index; p++)
                {
                    sumOsc += (double?)results[p].Oscillator;
                }

                smooth[i] = sumOsc / smoothPeriods;
            }
        }
        else if (movingAverageType is MaType.SMMA)
        {
            // initialize with unsmoothed value
            double? k = (double?)results[lookbackPeriods - 1].Oscillator;

            for (int i = lookbackPeriods - 1; i < size; i++)
            {
                k = (k == null) ? (double?)results[i].Oscillator : k; // reset if null

                k = ((k * (smoothPeriods - 1)) + (double?)results[i].Oscillator) / smoothPeriods;
                smooth[i] = k;
            }
        }
        else
        {
            return results;
        }

        // replace oscillator
        for (int i = 0; i < size; i++)
        {
            results[i].Oscillator = (smooth[i] != null) ? (decimal?)smooth[i] : null;
        }

        return results;
    }

    // parameter validation
    private static void ValidateStoch<TQuote>(
        IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        int signalPeriods,
        int smoothPeriods,
        decimal kFactor,
        decimal dFactor,
        MaType movingAverageType)
        where TQuote : IQuote
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Stochastic.");
        }

        if (signalPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                "Signal periods must be greater than 0 for Stochastic.");
        }

        if (smoothPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smoothPeriods), smoothPeriods,
                "Smooth periods must be greater than 0 for Stochastic.");
        }

        if (kFactor <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(kFactor), kFactor,
                "kFactor must be greater than 0 for Stochastic.");
        }

        if (dFactor <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(dFactor), dFactor,
                "dFactor must be greater than 0 for Stochastic.");
        }

        if (movingAverageType is not MaType.SMA and not MaType.SMMA)
        {
            throw new ArgumentOutOfRangeException(nameof(dFactor), dFactor,
                "Stochastic only supports SMA and SMMA moving average types.");
        }

        // check quotes
        int qtyHistory = quotes.Count();
        int minHistory = lookbackPeriods + smoothPeriods;
        if (qtyHistory < minHistory)
        {
            string message = "Insufficient quotes provided for Stochastic.  " +
                string.Format(
                    EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

            throw new BadQuotesException(nameof(quotes), message);
        }
    }
}
