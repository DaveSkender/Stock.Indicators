namespace Skender.Stock.Indicators;

// PARABOLIC SAR (SERIES)

public static partial class Indicator
{
    internal static List<ParabolicSarResult> CalcParabolicSar(
        this List<QuoteD> qdList,
        double accelerationStep,
        double maxAccelerationFactor,
        double initialFactor)
    {
        // check parameter arguments
        ParabolicSar.Validate(
            accelerationStep, maxAccelerationFactor, initialFactor);

        // initialize
        int length = qdList.Count;
        List<ParabolicSarResult> results = new(length);
        QuoteD q0;

        if (length == 0)
        {
            return results;
        }
        else
        {
            q0 = qdList[0];
        }

        double accelerationFactor = initialFactor;
        double extremePoint = q0.High;
        double priorSar = q0.Low;
        bool isRising = true;  // initial guess

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            QuoteD q = qdList[i];

            ParabolicSarResult r = new() { Timestamp = q.Timestamp };
            results.Add(r);

            // skip first one
            if (i == 0)
            {
                continue;
            }

            // was rising
            if (isRising)
            {
                double sar =
                    priorSar + (accelerationFactor * (extremePoint - priorSar));

                // SAR cannot be higher than last two lows
                if (i >= 2)
                {
                    double minLastTwo =
                        Math.Min(
                            qdList[i - 1].Low,
                            qdList[i - 2].Low);

                    sar = Math.Min(sar, minLastTwo);
                }

                // turn down
                if (q.Low < sar)
                {
                    r.IsReversal = true;
                    r.Sar = extremePoint;

                    isRising = false;
                    accelerationFactor = initialFactor;
                    extremePoint = q.Low;
                }

                // continue rising
                else
                {
                    r.IsReversal = false;
                    r.Sar = sar;

                    // new high extreme point
                    if (q.High > extremePoint)
                    {
                        extremePoint = q.High;
                        accelerationFactor =
                            Math.Min(
                                accelerationFactor + accelerationStep,
                                maxAccelerationFactor);
                    }
                }
            }

            // was falling
            else
            {
                double sar
                    = priorSar - (accelerationFactor * (priorSar - extremePoint));

                // SAR cannot be lower than last two highs
                if (i >= 2)
                {
                    double maxLastTwo = Math.Max(
                        qdList[i - 1].High,
                        qdList[i - 2].High);

                    sar = Math.Max(sar, maxLastTwo);
                }

                // turn up
                if (q.High > sar)
                {
                    r.IsReversal = true;
                    r.Sar = extremePoint;

                    isRising = true;
                    accelerationFactor = initialFactor;
                    extremePoint = q.High;
                }

                // continue falling
                else
                {
                    r.IsReversal = false;
                    r.Sar = sar;

                    // new low extreme point
                    if (q.Low < extremePoint)
                    {
                        extremePoint = q.Low;
                        accelerationFactor =
                            Math.Min(
                                accelerationFactor + accelerationStep,
                                maxAccelerationFactor);
                    }
                }
            }

            priorSar = (double)r.Sar;
        }

        // remove first trendline since it is an invalid guess
        int cutIndex = results.FindIndex(x => x.IsReversal is true);

        for (int d = 0; d <= cutIndex; d++)
        {
            ParabolicSarResult r = results[d];
            r.Sar = null;
            r.IsReversal = null;
        }

        return results;
    }
}
