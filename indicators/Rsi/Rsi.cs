using System;
using System.Collections.Generic;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // RELATIVE STRENGTH INDEX
        public static IEnumerable<RsiResult> GetRsi<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod = 14)
            where TQuote : IQuote
        {

            // convert history to basic format
            List<BasicData> bd = Cleaners.ConvertHistoryToBasic(history, "C");

            // calculate
            return CalcRsi(bd, lookbackPeriod);
        }


        private static IEnumerable<RsiResult> CalcRsi(List<BasicData> bdList, int lookbackPeriod = 14)
        {

            // check parameters
            ValidateRsi(bdList, lookbackPeriod);

            // initialize
            decimal lastValue = bdList[0].Value;
            decimal avgGain = 0m;
            decimal avgLoss = 0m;
            List<RsiResult> results = new List<RsiResult>(bdList.Count);

            // roll through history
            for (int i = 0; i < bdList.Count; i++)
            {
                BasicData h = bdList[i];
                int index = i + 1;

                RsiResult r = new RsiResult
                {
                    Date = h.Date,
                    Gain = (h.Value > lastValue) ? h.Value - lastValue : 0,
                    Loss = (h.Value < lastValue) ? lastValue - h.Value : 0
                };
                results.Add(r);
                lastValue = h.Value;

                // calculate RSI
                if (index > lookbackPeriod + 1)
                {
                    avgGain = (avgGain * (lookbackPeriod - 1) + r.Gain) / lookbackPeriod;
                    avgLoss = (avgLoss * (lookbackPeriod - 1) + r.Loss) / lookbackPeriod;

                    if (avgLoss > 0)
                    {
                        decimal rs = avgGain / avgLoss;
                        r.Rsi = 100 - (100 / (1 + rs));
                    }
                    else
                    {
                        r.Rsi = 100;
                    }
                }

                // initialize average gain
                else if (index == lookbackPeriod + 1)
                {
                    decimal sumGain = 0;
                    decimal sumLoss = 0;

                    for (int p = 1; p <= lookbackPeriod; p++)
                    {
                        RsiResult d = results[p];
                        sumGain += d.Gain;
                        sumLoss += d.Loss;
                    }
                    avgGain = sumGain / lookbackPeriod;
                    avgLoss = sumLoss / lookbackPeriod;

                    r.Rsi = (avgLoss > 0) ? 100 - (100 / (1 + (avgGain / avgLoss))) : 100;
                }
            }

            return results;
        }


        private static void ValidateRsi(List<BasicData> history, int lookbackPeriod)
        {

            // check parameters
            if (lookbackPeriod < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for RSI.");
            }

            // check history
            int qtyHistory = history.Count;
            int minHistory = lookbackPeriod + 50;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for RSI.  " +
                    string.Format(englishCulture,
                    "You provided {0} periods of history when at least {1} is required.  "
                    + "Since this uses a smoothing technique, "
                    + "we recommend you use at least {2} data points prior to the intended "
                    + "usage date for maximum precision.",
                    qtyHistory, minHistory, Math.Max(10 * lookbackPeriod, minHistory));

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }

}
