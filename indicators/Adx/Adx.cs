using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // AVERAGE DIRECTIONAL INDEX
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<AdxResult> GetAdx<TQuote>(
            this IEnumerable<TQuote> history,
            int lookbackPeriod = 14)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateAdx(history, lookbackPeriod);

            // initialize
            List<AdxResult> results = new(historyList.Count);
            List<AtrResult> atr = GetAtr(history, lookbackPeriod).ToList(); // get True Range info

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
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                AdxResult result = new()
                {
                    Date = h.Date
                };
                results.Add(result);

                // skip first period
                if (index == 1)
                {
                    prevHigh = h.High;
                    prevLow = h.Low;
                    continue;
                }

                decimal tr = (decimal)atr[i].Tr;

                decimal pdm1 = (h.High - prevHigh) > (prevLow - h.Low) ?
                    Math.Max(h.High - prevHigh, 0) : 0;

                decimal mdm1 = (prevLow - h.Low) > (h.High - prevHigh) ?
                    Math.Max(prevLow - h.Low, 0) : 0;

                prevHigh = h.High;
                prevLow = h.Low;

                // initialization period
                if (index <= lookbackPeriod + 1)
                {
                    sumTr += tr;
                    sumPdm += pdm1;
                    sumMdm += mdm1;
                }

                // skip DM initialization period
                if (index <= lookbackPeriod)
                {
                    continue;
                }


                // smoothed true range and directional movement
                decimal trs;
                decimal pdm;
                decimal mdm;

                if (index == lookbackPeriod + 1)
                {
                    trs = sumTr;
                    pdm = sumPdm;
                    mdm = sumMdm;
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

                if (trs == 0)
                {
                    continue;
                }

                // directional increments
                decimal pdi = 100 * pdm / trs;
                decimal mdi = 100 * mdm / trs;

                result.Pdi = pdi;
                result.Mdi = mdi;

                if (pdi + mdi == 0)
                {
                    continue;
                }

                // calculate ADX
                decimal dx = 100 * Math.Abs(pdi - mdi) / (pdi + mdi);
                decimal adx;

                if (index > 2 * lookbackPeriod)
                {
                    adx = (prevAdx * (lookbackPeriod - 1) + dx) / lookbackPeriod;
                    result.Adx = adx;
                    prevAdx = adx;
                }

                // initial ADX
                else if (index == 2 * lookbackPeriod)
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

            }

            return results;
        }


        private static void ValidateAdx<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 1 for ADX.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = 2 * lookbackPeriod + 100;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for ADX.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.  "
                    + "Since this uses a smoothing technique, "
                    + "we recommend you use at least 2×N+250 data points prior to the intended "
                    + "usage date for better precision.", qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
