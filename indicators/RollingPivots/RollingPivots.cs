using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // PIVOT POINTS
        public static IEnumerable<PivotPointsResult> GetRollingPivots<TQuote>(
            IEnumerable<TQuote> history,
            int windowPeriod,
            int offsetPeriod,
            PivotPointType pointType = PivotPointType.Standard)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateRollingPivots(history, windowPeriod, offsetPeriod);

            // initialize
            List<PivotPointsResult> results = new List<PivotPointsResult>(historyList.Count);

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];

                PivotPointsResult r = new PivotPointsResult
                {
                    Date = h.Date
                };

                if (i >= windowPeriod + offsetPeriod - 1)
                {
                    decimal windowOpen = h.Open;  // for Woodie only
                    decimal windowClose = historyList[i - offsetPeriod].Close;

                    // extreme values
                    int s = i - windowPeriod - offsetPeriod + 1;
                    TQuote hi = historyList[s];
                    decimal windowHigh = hi.High;
                    decimal windowLow = hi.Low;

                    for (int p = s; p <= i - offsetPeriod; p++)
                    {
                        TQuote d = historyList[p];
                        windowHigh = (d.High > windowHigh) ? d.High : windowHigh;
                        windowLow = (d.Low < windowLow) ? d.Low : windowLow;
                    }

                    // window pivot points
                    PivotPointsResult wp =
                        GetPivotPoint(pointType, windowOpen, windowHigh, windowLow, windowClose);

                    r.PP = wp.PP;

                    // support
                    r.S1 = wp.S1;
                    r.S2 = wp.S2;
                    r.S3 = wp.S3;
                    r.S4 = wp.S4;

                    // resistance
                    r.R1 = wp.R1;
                    r.R2 = wp.R2;
                    r.R3 = wp.R3;
                    r.R4 = wp.R4;
                }
                results.Add(r);
            }
            return results;
        }


        private static void ValidateRollingPivots<TQuote>(
            IEnumerable<TQuote> history,
            int windowPeriod,
            int offsetPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (windowPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(windowPeriod), windowPeriod,
                    "Window period must be greater than 0 for Rolling Pivot Points.");
            }

            if (offsetPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offsetPeriod), offsetPeriod,
                    "Offset period must be greater than 0 for Rolling Pivot Points.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = windowPeriod + offsetPeriod;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Rolling Pivot Points.  " +
                    string.Format(englishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }

    }
}