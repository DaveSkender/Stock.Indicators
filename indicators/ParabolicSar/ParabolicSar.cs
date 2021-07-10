using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // PARABOLIC SAR
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<ParabolicSarResult> GetParabolicSar<TQuote>(
            this IEnumerable<TQuote> quotes,
            decimal accelerationStep = (decimal)0.02,
            decimal maxAccelerationFactor = (decimal)0.2)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateParabolicSar(quotes, accelerationStep, maxAccelerationFactor);

            // initialize
            List<ParabolicSarResult> results = new(quotesList.Count);
            TQuote first = quotesList[0];

            decimal accelerationFactor = accelerationStep;
            decimal extremePoint = first.High;
            decimal priorSar = first.Low;
            bool isRising = true;  // initial guess

            // roll through quotes
            for (int i = 0; i < quotesList.Count; i++)
            {
                TQuote q = quotesList[i];

                ParabolicSarResult result = new()
                {
                    Date = q.Date
                };

                // skip first one
                if (i == 0)
                {
                    results.Add(result);
                    continue;
                }

                // was rising
                if (isRising)
                {
                    decimal currentSar = priorSar + accelerationFactor * (extremePoint - priorSar);

                    // turn down
                    if (q.Low < currentSar)
                    {
                        result.IsReversal = true;
                        result.Sar = extremePoint;

                        isRising = false;
                        accelerationFactor = accelerationStep;
                        extremePoint = q.Low;
                    }

                    // continue rising
                    else
                    {
                        result.IsReversal = false;
                        result.Sar = currentSar;

                        // SAR cannot be higher than last two lows
                        if (i >= 2)
                        {
                            decimal minLastTwo = Math.Min(quotesList[i - 1].Low, quotesList[i - 2].Low);

                            result.Sar = Math.Min((decimal)result.Sar, minLastTwo);
                        }
                        else
                        {
                            result.Sar = (decimal)result.Sar;
                        }

                        if (q.High > extremePoint)
                        {
                            extremePoint = q.High;
                            accelerationFactor =
                                Math.Min(accelerationFactor += accelerationStep, maxAccelerationFactor);
                        }
                    }
                }

                // was falling
                else
                {
                    decimal currentSar = priorSar - accelerationFactor * (priorSar - extremePoint);

                    // turn up
                    if (q.High > currentSar)
                    {
                        result.IsReversal = true;
                        result.Sar = extremePoint;

                        isRising = true;
                        accelerationFactor = accelerationStep;
                        extremePoint = q.High;
                    }

                    // continue falling
                    else
                    {
                        result.IsReversal = false;
                        result.Sar = currentSar;

                        // SAR cannot be lower than last two highs
                        if (i >= 2)
                        {
                            decimal maxLastTwo = Math.Max(quotesList[i - 1].High, quotesList[i - 2].High);

                            result.Sar = Math.Max((decimal)result.Sar, maxLastTwo);
                        }
                        else
                        {
                            result.Sar = (decimal)result.Sar;
                        }

                        if (q.Low < extremePoint)
                        {
                            extremePoint = q.Low;
                            accelerationFactor =
                                Math.Min(accelerationFactor += accelerationStep, maxAccelerationFactor);
                        }
                    }
                }

                priorSar = (decimal)result.Sar;

                results.Add(result);
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


        // remove recommended periods extensions
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
            decimal maxAccelerationFactor)
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
                string message = string.Format(
                    EnglishCulture,
                    "Acceleration Step must be smaller than provided Max Accleration Factor ({0}) for Parabolic SAR.",
                    maxAccelerationFactor);

                throw new ArgumentOutOfRangeException(nameof(accelerationStep), accelerationStep, message);
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = 2;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for Parabolic SAR.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
