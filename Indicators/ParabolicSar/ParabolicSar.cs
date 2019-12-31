using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // PARABOLIC SAR
        public static IEnumerable<ParabolicSarResult> GetParabolicSar(
            IEnumerable<Quote> history,
            decimal accelerationStep = (decimal)0.02,
            decimal maxAccelerationFactor = (decimal)0.2)
        {

            // clean quotes
            history = Cleaners.PrepareHistory(history);

            // initialize
            List<ParabolicSarResult> results = new List<ParabolicSarResult>();
            Quote first = history.Where(x => x.Index == 0).FirstOrDefault();

            decimal accelerationFactor = accelerationStep;
            decimal extremePoint = first.High;
            decimal priorSar = first.Low;
            bool isRising = true;

            // roll through history
            foreach (Quote h in history)
            {
                ParabolicSarResult result = new ParabolicSarResult
                {
                    Index = (int)h.Index,
                    Date = h.Date
                };

                // skip first one
                if (h.Index == 0)
                {
                    results.Add(result);
                    continue;
                }


                // was rising
                if (isRising)
                {
                    decimal currentSar = priorSar + accelerationFactor * (extremePoint - priorSar);

                    // turn down
                    if (h.Low < currentSar)
                    {
                        result.IsReversal = true;
                        result.Sar = extremePoint;

                        isRising = false;
                        accelerationFactor = accelerationStep;
                        extremePoint = h.Low;
                    }

                    // continue rising
                    else
                    {
                        result.IsReversal = false;
                        result.Sar = currentSar;

                        // SAR cannot be higher than last two lows
                        decimal minLastTwo = history.Where(x => x.Index >= h.Index - 2 && x.Index < h.Index).Select(x => x.Low).Min();
                        result.Sar = Math.Min((decimal)result.Sar, minLastTwo);

                        if (h.High > extremePoint)
                        {
                            extremePoint = h.High;
                            accelerationFactor = Math.Min(accelerationFactor += accelerationStep, maxAccelerationFactor);
                        }
                    }
                }

                // was falling
                else
                {
                    decimal currentSar = priorSar - accelerationFactor * (priorSar - extremePoint);

                    // turn up
                    if (h.High > currentSar)
                    {
                        result.IsReversal = true;
                        result.Sar = extremePoint;

                        isRising = true;
                        accelerationFactor = accelerationStep;
                        extremePoint = h.High;
                    }

                    // continue falling
                    else
                    {
                        result.IsReversal = false;
                        result.Sar = currentSar;

                        // SAR cannot be lower than last two highs
                        decimal maxLastTwo = history.Where(x => x.Index >= h.Index - 2 && x.Index < h.Index).Select(x => x.High).Max();
                        result.Sar = Math.Max((decimal)result.Sar, maxLastTwo);

                        if (h.Low < extremePoint)
                        {
                            extremePoint = h.Low;
                            accelerationFactor = Math.Min(accelerationFactor += accelerationStep, maxAccelerationFactor);
                        }
                    }
                }

                result.IsRising = isRising;
                priorSar = (decimal)result.Sar;

                results.Add(result);
            }

            return results;
        }

    }

}
