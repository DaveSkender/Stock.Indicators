namespace Skender.Stock.Indicators;

// STOCHASTIC OSCILLATOR (SERIES)

public static partial class Indicator
{
    private static List<StochResult> CalcStoch(
        this List<QuoteD> qdList,
        int lookbackPeriods,
        int signalPeriods,
        int smoothPeriods,
        double kFactor,
        double dFactor,
        MaType movingAverageType)
    {
        // check parameter arguments
        Stoch.Validate(
            lookbackPeriods, signalPeriods, smoothPeriods,
            kFactor, dFactor, movingAverageType);

        // initialize
        int length = qdList.Count;
        List<StochResult> results = new(length);

        double[] o = new double[length]; // %K oscillator (initial)
        double[] k = new double[length]; // %K oscillator (final)

        double prevK = double.NaN;
        double prevD = double.NaN;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            QuoteD q = qdList[i];

            // initial %K oscillator
            if (i >= lookbackPeriods - 1)
            {
                double highHigh = double.MinValue;
                double lowLow = double.MaxValue;
                bool isViable = true;

                for (int p = i - lookbackPeriods + 1; p <= i; p++)
                {
                    QuoteD x = qdList[p];

                    if (double.IsNaN(x.High)
                     || double.IsNaN(x.Low)
                     || double.IsNaN(x.Close))
                    {
                        isViable = false;
                        break;
                    }

                    if (x.High > highHigh)
                    {
                        highHigh = x.High;
                    }

                    if (x.Low < lowLow)
                    {
                        lowLow = x.Low;
                    }
                }

                o[i] = !isViable
                     ? double.NaN
                     : highHigh - lowLow != 0
                     ? 100 * (q.Close - lowLow) / (highHigh - lowLow)
                     : 0;
            }
            else
            {
                o[i] = double.NaN;
            }

            // final %K oscillator, keep original
            if (smoothPeriods <= 1)
            {
                k[i] = o[i];
            }

            // final %K oscillator, if smoothed
            else if (i >= smoothPeriods)
            {
                k[i] = double.NaN;

                switch (movingAverageType)
                {
                    // SMA case
                    case MaType.SMA:
                        {
                            double sum = 0;
                            for (int p = i - smoothPeriods + 1; p <= i; p++)
                            {
                                sum += o[p];
                            }

                            k[i] = sum / smoothPeriods;
                            break;
                        }

                    // SMMA case
                    case MaType.SMMA:
                        {
                            // re/initialize
                            if (double.IsNaN(prevK))
                            {
                                prevK = o[i];
                            }

                            k[i] = (prevK * (smoothPeriods - 1) + o[i]) / smoothPeriods;
                            prevK = k[i];
                            break;
                        }

                    default:
                        throw new InvalidOperationException(
                            "Invalid Stochastic moving average type.");
                }
            }
            else
            {
                k[i] = double.NaN;
            }

            double oscillator = k[i];
            double signal;


            // %D signal line
            if (signalPeriods <= 1)
            {
                signal = oscillator;
            }
            else if (i >= signalPeriods)
            {
                switch (movingAverageType)
                {
                    // SMA case
                    // TODO: || double.IsNaN(prevD) to re/initialize SMMA?
                    case MaType.SMA:
                        {
                            double sum = 0;
                            for (int p = i - signalPeriods + 1; p <= i; p++)
                            {
                                sum += k[p];
                            }

                            signal = sum / signalPeriods;
                            break;
                        }

                    // SMMA case
                    case MaType.SMMA:
                        {
                            // re/initialize
                            if (double.IsNaN(prevD))
                            {
                                prevD = k[i];
                            }

                            double d = (prevD * (signalPeriods - 1) + k[i]) / signalPeriods;
                            signal = d;
                            prevD = d;
                            break;
                        }

                    default:
                        throw new InvalidOperationException("Invalid Stochastic moving average type.");
                }
            }
            else
            {
                signal = double.NaN;
            }

            results.Add(new(
                Timestamp: q.Timestamp,
                Oscillator: oscillator.NaN2Null(),
                Signal: signal.NaN2Null(),
                PercentJ: (kFactor * oscillator - dFactor * signal).NaN2Null()));
        }
        return results;
    }
}
