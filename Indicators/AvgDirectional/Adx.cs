using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // AVERAGE DIRECTIONAL INDEX
        public static IEnumerable<AdxResult> GetAdx(IEnumerable<Quote> history, int lookbackPeriod = 14)
        {

            // clean quotes
            history = Cleaners.PrepareHistory(history);

            // check exceptions
            int qtyHistory = history.Count();
            int minHistory = 2 * lookbackPeriod + 1;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for ADX.  " +
                        string.Format("You provided {0} periods of history when {1} is required.  "
                          + "Since this uses a smoothing technique, "
                          + "we recommend you use at least 250 data points prior to the intended "
                          + "usage date for maximum precision.", qtyHistory, minHistory));
            }

            // initialize results and working variables
            IEnumerable<AtrResult> atrResults = GetAtr(history, lookbackPeriod); // uses True Range value
            List<AdxResult> results = new List<AdxResult>();

            decimal prevHigh = 0;
            decimal prevLow = 0;
            decimal prevTrs = 0; // smoothed
            decimal prevPdm = 0;
            decimal prevMdm = 0;
            decimal prevAdx = 0;

            decimal sumTr = 0;
            decimal sumPdm = 0;
            decimal sumMdm = 0;
            decimal sumDx = 0;

            // roll through history
            foreach (Quote h in history)
            {

                AdxResult result = new AdxResult
                {
                    Index = (int)h.Index,
                    Date = h.Date
                };

                // skip first period
                if (h.Index == 1)
                {
                    results.Add(result);
                    prevHigh = h.High;
                    prevLow = h.Low;
                    continue;
                }

                decimal tr = (decimal)atrResults.Where(x => x.Index == h.Index).FirstOrDefault().Tr;
                decimal pdm1 = (h.High - prevHigh) > (prevLow - h.Low) ? Math.Max(h.High - prevHigh, 0) : 0;
                decimal mdm1 = (prevLow - h.Low) > (h.High - prevHigh) ? Math.Max(prevLow - h.Low, 0) : 0;

                prevHigh = h.High;
                prevLow = h.Low;

                // initialization period
                if (h.Index <= lookbackPeriod + 1)
                {
                    sumTr += tr;
                    sumPdm += pdm1;
                    sumMdm += mdm1;
                }

                // skip DM initialization period
                if (h.Index <= lookbackPeriod)
                {
                    results.Add(result);
                    continue;
                }


                // smoothed true range and directional movement
                decimal trs;
                decimal pdm;
                decimal mdm;

                if (h.Index == lookbackPeriod + 1)
                {
                    trs = sumTr / lookbackPeriod;
                    pdm = sumPdm / lookbackPeriod;
                    mdm = sumMdm / lookbackPeriod;
                }
                else
                {
                    trs = prevTrs - (prevTrs / lookbackPeriod) + tr;
                    pdm = prevPdm - (prevPdm / lookbackPeriod) + pdm1;
                    mdm = prevMdm - (prevMdm / lookbackPeriod) + mdm1;
                }

                prevTrs = trs;
                prevPdm = pdm;
                prevMdm = mdm;


                // directional increments
                decimal pdi = 100 * pdm / trs;
                decimal mdi = 100 * mdm / trs;
                decimal dx = 100 * Math.Abs(pdi - mdi) / (pdi + mdi);

                result.Pdi = pdi;
                result.Mdi = mdi;


                // calculate ADX
                decimal adx;

                if (h.Index > 2 * lookbackPeriod)
                {
                    adx = (prevAdx * (lookbackPeriod - 1) + dx) / lookbackPeriod;
                    result.Adx = adx;
                    prevAdx = adx;
                }

                // initial ADX
                else if (h.Index == 2 * lookbackPeriod)
                {
                    sumDx += dx;
                    adx = sumDx / lookbackPeriod;
                    result.Adx = adx;
                    prevAdx = adx;
                }

                // ADX initialization period
                else
                {
                    sumDx += dx;
                }

                results.Add(result);
            }

            return results;
        }

    }
}
