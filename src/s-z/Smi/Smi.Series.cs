namespace Skender.Stock.Indicators;

// STOCHASTIC MOMENTUM INDEX (SERIES)

public static partial class Indicator
{
    private static List<SmiResult> CalcSmi(
        this List<QuoteD> qdList,
        int lookbackPeriods,
        int firstSmoothPeriods,
        int secondSmoothPeriods,
        int signalPeriods)
    {
        // check parameter arguments
        Smi.Validate(
            lookbackPeriods,
            firstSmoothPeriods,
            secondSmoothPeriods,
            signalPeriods);

        // initialize
        int length = qdList.Count;
        List<SmiResult> results = new(length);

        double k1 = 2d / (firstSmoothPeriods + 1);
        double k2 = 2d / (secondSmoothPeriods + 1);
        double kS = 2d / (signalPeriods + 1);

        double lastSmEma1 = 0;
        double lastSmEma2 = 0;
        double lastHlEma1 = 0;
        double lastHlEma2 = 0;
        double lastSignal = 0;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = qdList[i];

            double smi;
            double signal;

            if (i >= lookbackPeriods - 1)
            {
                double hH = double.MinValue;
                double lL = double.MaxValue;

                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    QuoteD x = qdList[p];

                    if (x.High > hH)
                    {
                        hH = x.High;
                    }

                    if (x.Low < lL)
                    {
                        lL = x.Low;
                    }
                }

                double sm = q.Close - 0.5d * (hH + lL);
                double hl = hH - lL;

                // initialize last EMA values
                // TODO: update healing, without requiring specific indexing
                if (i == lookbackPeriods - 1)
                {
                    lastSmEma1 = sm;
                    lastSmEma2 = lastSmEma1;
                    lastHlEma1 = hl;
                    lastHlEma2 = lastHlEma1;
                }

                // first smoothing
                double smEma1 = lastSmEma1 + k1 * (sm - lastSmEma1);
                double hlEma1 = lastHlEma1 + k1 * (hl - lastHlEma1);

                // second smoothing
                double smEma2 = lastSmEma2 + k2 * (smEma1 - lastSmEma2);
                double hlEma2 = lastHlEma2 + k2 * (hlEma1 - lastHlEma2);

                // stochastic momentum index
                smi = 100 * (smEma2 / (0.5 * hlEma2));

                // initialize signal line
                // TODO: update healing, without requiring specific indexing
                if (i == lookbackPeriods - 1)
                {
                    lastSignal = smi;
                }

                // signal line
                signal = lastSignal + kS * (smi - lastSignal);

                // carryover values
                lastSmEma1 = smEma1;
                lastSmEma2 = smEma2;
                lastHlEma1 = hlEma1;
                lastHlEma2 = hlEma2;
                lastSignal = signal;
            }
            else
            {
                smi = double.NaN;
                signal = double.NaN;
            }

            results.Add(new(
                Timestamp: q.Timestamp,
                Smi: smi.NaN2Null(),
                Signal: signal.NaN2Null()));
        }

        return results;
    }
}
