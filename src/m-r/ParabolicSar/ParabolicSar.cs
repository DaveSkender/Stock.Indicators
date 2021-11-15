using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
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
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateParabolicSar(
                quotes, accelerationStep, maxAccelerationFactor, initialFactor);

            // initialize
            List<ParabolicSarResult> results = new(quotesList.Count);
            TQuote first = quotesList[0];

            decimal accelerationFactor = initialFactor;
            decimal extremePoint = first.High;
            decimal priorSar = first.Low;
            bool isRising = true;  // initial guess

            // roll through quotes
            for (int i = 0; i < quotesList.Count; i++)
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
                        priorSar + accelerationFactor * (extremePoint - priorSar);

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
                            accelerationFactor =
                                Math.Min(
                                    accelerationFactor += accelerationStep,
                                    maxAccelerationFactor);
                        }
                    }
                }

                // was falling
                else
                {
                    decimal sar
                        = priorSar - accelerationFactor * (priorSar - extremePoint);

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
                            accelerationFactor =
                                Math.Min(
                                    accelerationFactor += accelerationStep,
                                    maxAccelerationFactor);
                        }
                    }
                }

                priorSar = (decimal)r.Sar;

                //Console.WriteLine($"{i},{r.IsReversal},{extremePoint},{accelerationFactor},{r.Sar:N4}");
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


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<ParabolicSarResult> RemoveWarmupPeriods(
            this IEnumerable<ParabolicSarResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.Sar != null);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateParabolicSar<TQuote>(
            IEnumerable<TQuote> quotes,
            decimal accelerationStep,
            decimal maxAccelerationFactor,
            decimal initialFactor)
            where TQuote : IQuote
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
                string message = string.Format(EnglishCulture,
                    "Acceleration Step must be smaller than provided Max Accleration Factor ({0}) for Parabolic SAR.",
                    maxAccelerationFactor);

                throw new ArgumentOutOfRangeException(nameof(accelerationStep), accelerationStep, message);
            }

            if (initialFactor <= 0 || initialFactor >= maxAccelerationFactor)
            {
                throw new ArgumentOutOfRangeException(nameof(initialFactor), initialFactor,
                    "Initial Step must be greater than 0 and less than Max Acceleration Factor for Parabolic SAR.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = 2;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for Parabolic SAR.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
