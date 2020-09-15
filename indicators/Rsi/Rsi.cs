using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // RELATIVE STRENGTH INDEX
        public static IEnumerable<RsiResult> GetRsi(IEnumerable<Quote> history, int lookbackPeriod = 14)
        {

            // convert history to basic format
            IEnumerable<BasicData> bd = Cleaners.ConvertHistoryToBasic(history, "C");

            // calculate
            return CalcRsi(bd, lookbackPeriod);
        }


        private static IEnumerable<RsiResult> CalcRsi(IEnumerable<BasicData> basicData, int lookbackPeriod = 14)
        {

            // clean data
            List<BasicData> bdList = Cleaners.PrepareBasicData(basicData).ToList();

            // check parameters
            ValidateRsi(basicData, lookbackPeriod);

            // initialize
            decimal lastValue = bdList[0].Value;
            decimal avgGain = 0m;
            decimal avgLoss = 0m;
            List<RsiResult> results = new List<RsiResult>();

            // roll through history
            for (int i = 0; i < bdList.Count; i++)
            {
                BasicData h = bdList[i];

                RsiResult r = new RsiResult
                {
                    Index = (int)h.Index,
                    Date = h.Date,
                    Gain = (h.Value > lastValue) ? h.Value - lastValue : 0,
                    Loss = (h.Value < lastValue) ? lastValue - h.Value : 0
                };
                results.Add(r);
                lastValue = h.Value;

                // calculate RSI
                if (h.Index > lookbackPeriod + 1)
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
                else if (h.Index == lookbackPeriod + 1)
                {
                    decimal sumGain = 0;
                    decimal sumLoss = 0;

                    for (int p = 0; p < lookbackPeriod; p++)
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


        private static void ValidateRsi(IEnumerable<BasicData> basicData, int lookbackPeriod)
        {

            // check parameters
            if (lookbackPeriod <= 1)
            {
                throw new BadParameterException("Lookback period must be greater than 1 for RSI.");
            }

            // check history
            int qtyHistory = basicData.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for RSI.  " +
                        string.Format(englishCulture,
                        "You provided {0} periods of history when at least {1} is required.  "
                          + "Since this uses a smoothing technique, "
                          + "we recommend you use at least 250 data points prior to the intended "
                          + "usage date for maximum precision.", qtyHistory, minHistory));
            }
        }
    }

}
