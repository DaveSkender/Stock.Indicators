﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // AVERAGE TRUE RANGE
        public static IEnumerable<AtrResult> GetAtr(IEnumerable<Quote> history, int lookbackPeriod = 14)
        {

            // clean quotes
            history = Cleaners.PrepareHistory(history);

            // check exceptions
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod + 1;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for ATR.  " +
                        string.Format("You provided {0} periods of history when {1} is required.  "
                        , qtyHistory, minHistory));
            }

            // initialize results
            List<AtrResult> results = new List<AtrResult>();
            decimal prevAtr = 0;
            decimal prevClose = 0;
            decimal highMinusPrevClose = 0;
            decimal lowMinusPrevClose = 0;
            decimal sumTr = 0;

            // roll through history
            foreach (Quote h in history)
            {

                AtrResult result = new AtrResult
                {
                    Index = (int)h.Index,
                    Date = h.Date
                };

                if (h.Index > 1)
                {
                    highMinusPrevClose = Math.Abs(h.High - prevClose);
                    lowMinusPrevClose = Math.Abs(h.Low - prevClose);
                }

                decimal tr = Math.Max((h.High - h.Low), Math.Max(highMinusPrevClose, lowMinusPrevClose));
                result.Tr = tr;

                if (h.Index > lookbackPeriod)
                {
                    // calculate ATR
                    result.Atr = (prevAtr * (lookbackPeriod - 1) + tr) / lookbackPeriod;
                    prevAtr = (decimal)result.Atr;
                }
                else if (h.Index == lookbackPeriod)
                {
                    // initialize ATR
                    sumTr += tr;
                    result.Atr = sumTr / lookbackPeriod;
                    prevAtr = (decimal)result.Atr;
                }
                else
                {
                    // only used for periods before ATR initialization
                    sumTr += tr;
                }

                results.Add(result);
                prevClose = h.Close;
            }

            return results;
        }

    }
}
