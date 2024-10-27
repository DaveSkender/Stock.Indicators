namespace Skender.Stock.Indicators;

// PARABOLIC SAR (SERIES)

public static partial class ParabolicSar
{
    public static IReadOnlyList<ParabolicSarResult> ToParabolicSar<TQuote>(
    this IReadOnlyList<TQuote> quotes,
    double accelerationStep = 0.02,
    double maxAccelerationFactor = 0.2)
    where TQuote : IQuote => quotes
        .ToQuoteDList()
        .CalcParabolicSar(
            accelerationStep,
            maxAccelerationFactor,
            accelerationStep);

    public static IReadOnlyList<ParabolicSarResult> GetParabolicSar<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        double accelerationStep,
        double maxAccelerationFactor,
        double initialFactor)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcParabolicSar(
                accelerationStep,
                maxAccelerationFactor,
                initialFactor);

    private static List<ParabolicSarResult> CalcParabolicSar(
        this List<QuoteD> source,
        double accelerationStep,
        double maxAccelerationFactor,
        double initialFactor)
    {
        // check parameter arguments
        ParabolicSar.Validate(
            accelerationStep, maxAccelerationFactor, initialFactor);

        // initialize
        int length = source.Count;
        List<ParabolicSarResult> results = new(length);

        if (length == 0)
        {
            return results;
        }

        QuoteD q0 = source[0];

        double accelerationFactor = initialFactor;
        double extremePoint = q0.High;
        double priorSar = q0.Low;
        bool isRising = true;  // initial guess

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = source[i];

            bool? isReversal;
            double psar;

            // skip first one
            if (i == 0)
            {
                results.Add(new(q.Timestamp));
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
                            source[i - 1].Low,
                            source[i - 2].Low);

                    sar = Math.Min(sar, minLastTwo);
                }

                // turn down
                if (q.Low < sar)
                {
                    isReversal = true;
                    psar = extremePoint;

                    isRising = false;
                    accelerationFactor = initialFactor;
                    extremePoint = q.Low;
                }

                // continue rising
                else
                {
                    isReversal = false;
                    psar = sar;

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
                        source[i - 1].High,
                        source[i - 2].High);

                    sar = Math.Max(sar, maxLastTwo);
                }

                // turn up
                if (q.High > sar)
                {
                    isReversal = true;
                    psar = extremePoint;

                    isRising = true;
                    accelerationFactor = initialFactor;
                    extremePoint = q.High;
                }

                // continue falling
                else
                {
                    isReversal = false;
                    psar = sar;

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

            results.Add(new ParabolicSarResult(
                Timestamp: q.Timestamp,
                Sar: psar.NaN2Null(),
                IsReversal: isReversal));

            priorSar = psar;
        }

        // remove first trendline since it is an invalid guess
        int cutIndex = results.FindIndex(x => x.IsReversal == true);

        cutIndex = cutIndex < 0 ? length - 1 : cutIndex;

        for (int d = 0; d <= cutIndex; d++)
        {
            ParabolicSarResult r = results[d] with {
                Sar = null,
                IsReversal = null
            };

            results[d] = r;
        }

        return results;
    }
}
