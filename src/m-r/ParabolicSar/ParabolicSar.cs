namespace Skender.Stock.Indicators;
#nullable disable

public static partial class Indicator
{
    // PARABOLIC SAR
    /// <include file='./info.xml' path='indicator/type[@name="Standard"]/*' />
    ///
    public static IEnumerable<ParabolicSarResult> GetParabolicSar<TQuote>(
        this IEnumerable<TQuote> quotes,
        decimal accelerationStep = 0.02m,
        decimal maxAccelerationFactor = 0.2m)
        where TQuote : IQuote
    {
        return quotes.GetParabolicSar(
            accelerationStep,
            maxAccelerationFactor,
            accelerationStep);
    }

    /// <include file='./info.xml' path='indicator/type[@name="Extended"]/*' />
    ///
    public static IEnumerable<ParabolicSarResult> GetParabolicSar<TQuote>(
        this IEnumerable<TQuote> quotes,
        decimal accelerationStep,
        decimal maxAccelerationFactor,
        decimal initialFactor)
        where TQuote : IQuote
    {
        // sort quotes
        List<TQuote> quotesList = quotes.SortToList();

        // check parameter arguments
        ValidateParabolicSar(
            accelerationStep, maxAccelerationFactor, initialFactor);

        // initialize
        int length = quotesList.Count;
        List<ParabolicSarResult> results = new(length);
        TQuote q0;

        if (length == 0)
        {
            return results;
        }
        else
        {
            q0 = quotesList[0];
        }

        decimal accelerationFactor = initialFactor;
        decimal extremePoint = q0.High;
        decimal priorSar = q0.Low;
        bool isRising = true;  // initial guess

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            TQuote q = quotesList[i];

            ParabolicSarResult r = new()
            {
                Date = q.Date
            };
            results.Add(r);

            // skip first one
            if (i == 0)
            {
                continue;
            }

            // was rising
            if (isRising)
            {
                decimal sar =
                    priorSar + (accelerationFactor * (extremePoint - priorSar));

                // SAR cannot be higher than last two lows
                if (i >= 2)
                {
                    decimal minLastTwo =
                        Math.Min(
                            quotesList[i - 1].Low,
                            quotesList[i - 2].Low);

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

                        decimal nextFactor = accelerationFactor + accelerationStep;

                        // do not exceed max allowed
                        accelerationFactor =
                            Math.Min(nextFactor, maxAccelerationFactor);
                    }
                }
            }

            // was falling
            else
            {
                decimal sar
                    = priorSar - (accelerationFactor * (priorSar - extremePoint));

                // SAR cannot be lower than last two highs
                if (i >= 2)
                {
                    decimal maxLastTwo = Math.Max(
                        quotesList[i - 1].High,
                        quotesList[i - 2].High);

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

                        decimal nextFactor = accelerationFactor + accelerationStep;

                        // do not exceed max allowed
                        accelerationFactor =
                            Math.Min(nextFactor, maxAccelerationFactor);
                    }
                }
            }

            priorSar = (decimal)r.Sar;
        }

        // remove first trend to reversal, since it is an invalid guess
        ParabolicSarResult firstReversal = results
            .Where(x => x.IsReversal == true)
            .OrderBy(x => x.Date)
            .FirstOrDefault();

        if (firstReversal != null)
        {
            int cutIndex = results.IndexOf(firstReversal);

            for (int d = 0; d <= cutIndex; d++)
            {
                ParabolicSarResult r = results[d];
                r.Sar = null;
                r.IsReversal = null;
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateParabolicSar(
        decimal accelerationStep,
        decimal maxAccelerationFactor,
        decimal initialFactor)
    {
        // check parameter arguments
        if (accelerationStep <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(accelerationStep), accelerationStep,
                "Acceleration Step must be greater than 0 for Parabolic SAR.");
        }

        if (maxAccelerationFactor <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxAccelerationFactor), maxAccelerationFactor,
                "Max Acceleration Factor must be greater than 0 for Parabolic SAR.");
        }

        if (accelerationStep > maxAccelerationFactor)
        {
            string message = string.Format(
                EnglishCulture,
                "Acceleration Step must be smaller than provided Max Accleration Factor ({0}) for Parabolic SAR.",
                maxAccelerationFactor);

            throw new ArgumentOutOfRangeException(nameof(accelerationStep), accelerationStep, message);
        }

        if (initialFactor <= 0 || initialFactor >= maxAccelerationFactor)
        {
            throw new ArgumentOutOfRangeException(nameof(initialFactor), initialFactor,
                "Initial Step must be greater than 0 and less than Max Acceleration Factor for Parabolic SAR.");
        }
    }
}
