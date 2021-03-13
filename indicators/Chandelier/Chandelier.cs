using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // CHANDELIER EXIT
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<ChandelierResult> GetChandelier<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod = 22,
            decimal multiplier = 3.0m,
            ChandelierType type = ChandelierType.Long)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateChandelier(history, lookbackPeriod, multiplier);

            // initialize
            List<ChandelierResult> results = new(historyList.Count);
            List<AtrResult> atrResult = GetAtr(history, lookbackPeriod).ToList();  // uses ATR

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                ChandelierResult result = new()
                {
                    Date = h.Date
                };

                // add exit values
                if (index >= lookbackPeriod)
                {

                    decimal atr = (decimal)atrResult[i].Atr;

                    switch (type)
                    {
                        case ChandelierType.Long:

                            decimal maxHigh = 0;
                            for (int p = index - lookbackPeriod; p < index; p++)
                            {
                                TQuote d = historyList[p];
                                if (d.High > maxHigh)
                                {
                                    maxHigh = d.High;
                                }
                            }

                            result.ChandelierExit = maxHigh - atr * multiplier;
                            break;

                        case ChandelierType.Short:

                            decimal minLow = decimal.MaxValue;
                            for (int p = index - lookbackPeriod; p < index; p++)
                            {
                                TQuote d = historyList[p];
                                if (d.Low < minLow)
                                {
                                    minLow = d.Low;
                                }
                            }

                            result.ChandelierExit = minLow + atr * multiplier;
                            break;

                        default:
                            break;
                    }
                }

                results.Add(result);
            }

            return results;
        }


        private static void ValidateChandelier<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod,
            decimal multiplier)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for Chandelier Exit.");
            }

            if (multiplier <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(multiplier), multiplier,
                    "Multiplier must be greater than 0 for Chandelier Exit.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod + 1;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Chandelier Exit.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
