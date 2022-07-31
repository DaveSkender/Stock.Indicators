namespace Skender.Stock.Indicators;

// STOCHASTIC OSCILLATOR (SERIES)
public static partial class Indicator
{
    internal static List<StochResult> CalcStoch(
        this List<QuoteD> qdList,
        int lookbackPeriods,
        int signalPeriods,
        int smoothPeriods,
        double kFactor,
        double dFactor,
        MaType movingAverageType)
    {
        // check parameter arguments
        ValidateStoch(
            lookbackPeriods, signalPeriods, smoothPeriods,
            kFactor, dFactor, movingAverageType);

        // initialize
        int length = qdList.Count;
        List<StochResult> results = new(length);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            QuoteD q = qdList[i];

            StochResult r = new(q.Date);
            results.Add(r);

            if (i + 1 >= lookbackPeriods)
            {
                double highHigh = double.MinValue;
                double lowLow = double.MaxValue;

                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    QuoteD x = qdList[p];

                    if (x.High > highHigh)
                    {
                        highHigh = x.High;
                    }

                    if (x.Low < lowLow)
                    {
                        lowLow = x.Low;
                    }
                }

                r.Oscillator = lowLow != highHigh
                    ? 100 * (q.Close - lowLow) / (highHigh - lowLow)
                    : 0;

                // reclaim nulls from NaNs
                r.Oscillator = NullMath.NaN2Null(r.Oscillator);
            }
        }

        // smooth the oscillator
        if (smoothPeriods > 1)
        {
            results = SmoothOscillator(
                results, length, lookbackPeriods, smoothPeriods, movingAverageType);
        }

        // handle insufficient length
        if (length < lookbackPeriods - 1)
        {
            return results;
        }

        // signal (%D) and %J
        int signalIndex = lookbackPeriods + smoothPeriods + signalPeriods - 2;
        double? s = results[lookbackPeriods - 1].Oscillator;

        for (int i = lookbackPeriods - 1; i < length; i++)
        {
            StochResult r = results[i];

            // add signal

            if (signalPeriods <= 1)
            {
                r.Signal = r.Oscillator;
            }

            // SMA case
            else if (i + 1 >= signalIndex && movingAverageType is MaType.SMA)
            {
                double? sumOsc = 0;
                for (int p = i + 1 - signalPeriods; p <= i; p++)
                {
                    StochResult x = results[p];
                    sumOsc += x.Oscillator;
                }

                r.Signal = sumOsc / signalPeriods;
            }

            // SMMA case
            else if (i >= lookbackPeriods - 1 && movingAverageType is MaType.SMMA)
            {
                s = (s == null) ? results[i].Oscillator : s; // reset if null

                s = ((s * (signalPeriods - 1)) + results[i].Oscillator) / signalPeriods;
                r.Signal = s;
            }

            // %J
            r.PercentJ = (kFactor * r.Oscillator) - (dFactor * r.Signal);
        }

        return results;
    }

    // internals
    private static List<StochResult> SmoothOscillator(
        List<StochResult> results,
        int length,
        int lookbackPeriods,
        int smoothPeriods,
        MaType movingAverageType)
    {
        // temporarily store interim smoothed oscillator
        double?[] smooth = new double?[length]; // smoothed value

        if (movingAverageType is MaType.SMA)
        {
            int smoothIndex = lookbackPeriods + smoothPeriods - 2;

            for (int i = smoothIndex; i < length; i++)
            {
                double? sumOsc = 0;
                for (int p = i + 1 - smoothPeriods; p <= i; p++)
                {
                    sumOsc += results[p].Oscillator;
                }

                smooth[i] = sumOsc / smoothPeriods;
            }
        }
        else if (movingAverageType is MaType.SMMA)
        {
            // initialize with unsmoothed value
            double? k = results[lookbackPeriods - 1].Oscillator;

            for (int i = lookbackPeriods - 1; i < length; i++)
            {
                k = (k == null) ? results[i].Oscillator : k; // reset if null

                k = ((k * (smoothPeriods - 1)) + results[i].Oscillator) / smoothPeriods;
                smooth[i] = k;
            }
        }
        else
        {
            return results;
        }

        // replace oscillator
        for (int i = 0; i < length; i++)
        {
            results[i].Oscillator = (smooth[i] != null) ? smooth[i] : null;
        }

        return results;
    }

    // parameter validation
    private static void ValidateStoch(
        int lookbackPeriods,
        int signalPeriods,
        int smoothPeriods,
        double kFactor,
        double dFactor,
        MaType movingAverageType)
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
    }
}
