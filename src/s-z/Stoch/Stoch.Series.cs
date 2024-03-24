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

            StochResult r = new() { Timestamp = q.Timestamp };
            results.Add(r);

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
                     : lowLow != highHigh
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

                if (movingAverageType is MaType.SMA)  // TODO: || double.IsNaN(prevK) to re/initialize SMMA?
                {
                    double sum = 0;
                    for (int p = i - smoothPeriods + 1; p <= i; p++)
                    {
                        sum += o[p];
                    }

                    k[i] = sum / smoothPeriods;
                }

                else if (movingAverageType is MaType.SMMA)
                {
                    // re/initialize
                    if (double.IsNaN(prevK))
                    {
                        prevK = o[i];
                    }

                    k[i] = ((prevK * (smoothPeriods - 1)) + o[i]) / smoothPeriods;
                    prevK = k[i];
                }
                else
                {
                    throw new InvalidOperationException("Invalid Stochastic moving average type.");
                }
            }
            else
            {
                k[i] = double.NaN;
            }

            r.Oscillator = k[i].NaN2Null();


            // %D signal line
            if (signalPeriods <= 1)
            {
                r.Signal = r.Oscillator;
            }
            else if (i >= signalPeriods)
            {

                // SMA case
                if (movingAverageType is MaType.SMA)  // TODO: || double.IsNaN(prevD) to re/initialize SMMA?
                {
                    double sum = 0;
                    for (int p = i - signalPeriods + 1; p <= i; p++)
                    {
                        sum += k[p];
                    }

                    r.Signal = (sum / signalPeriods).NaN2Null();
                }

                // SMMA case
                else if (movingAverageType is MaType.SMMA)
                {
                    // re/initialize
                    if (double.IsNaN(prevD))
                    {
                        prevD = k[i];
                    }

                    double d = ((prevD * (signalPeriods - 1)) + k[i]) / signalPeriods;
                    r.Signal = d.NaN2Null();
                    prevD = d;
                }

                else
                {
                    throw new InvalidOperationException("Invalid Stochastic moving average type.");
                }
            }

            // %J profile
            r.PercentJ = (kFactor * r.Oscillator) - (dFactor * r.Signal);

        }
        return results;
    }
}
