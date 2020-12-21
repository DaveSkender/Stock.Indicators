using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // PIVOT POINTS
        public static IEnumerable<PivotPointsResult> GetPivotPoints<TQuote>(
            IEnumerable<TQuote> history,
            PeriodSize windowSize,
            PivotPointType pointType = PivotPointType.Standard)
            where TQuote : IQuote
        {

            // clean quotes
            List<TQuote> historyList = history.Sort();

            // check parameters
            ValidatePivotPoints(history, windowSize);

            // initialize
            List<PivotPointsResult> results = new List<PivotPointsResult>(historyList.Count);
            PivotPointsResult windowPoint = new PivotPointsResult();

            TQuote h0 = historyList[0];
            int windowId = GetWindowNumber(h0.Date, windowSize);
            int windowEval;
            bool firstWindow = true;

            decimal windowHigh = h0.High;
            decimal windowLow = h0.Low;
            decimal windowOpen = h0.Open;
            decimal windowClose = h0.Close;

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];

                PivotPointsResult r = new PivotPointsResult
                {
                    Date = h.Date
                };

                // new window evaluation
                windowEval = GetWindowNumber(h.Date, windowSize);

                //Console.WriteLine("{0},{1:N4},{2:N4},{3:N4},{4:N4}", i + 1, windowOpen, windowHigh, windowLow, windowClose);

                if (windowEval != windowId)
                {
                    windowId = windowEval;
                    firstWindow = false;

                    // set new levels
                    windowPoint = GetPivotPoint(pointType, windowOpen, windowHigh, windowLow, windowClose);

                    // reset window min/max thresholds
                    windowOpen = h.Open;
                    windowHigh = h.High;
                    windowLow = h.Low;
                }

                // add levels
                if (!firstWindow)
                {
                    // pivot point
                    r.PP = windowPoint.PP;

                    // support
                    r.S1 = windowPoint.S1;
                    r.S2 = windowPoint.S2;
                    r.S3 = windowPoint.S3;
                    r.S4 = windowPoint.S4;

                    // resistance
                    r.R1 = windowPoint.R1;
                    r.R2 = windowPoint.R2;
                    r.R3 = windowPoint.R3;
                    r.R4 = windowPoint.R4;
                }


                //Console.WriteLine("{0},{1:N4},{2:N4},{3:N4},{4:N4},{5:N4}", i + 1, r.PP, r.S1, r.S2, r.R1, r.R2);
                results.Add(r);

                // capture window threholds (for next iteration)
                windowHigh = (h.High > windowHigh) ? h.High : windowHigh;
                windowLow = (h.Low < windowLow) ? h.Low : windowLow;
                windowClose = h.Close;
            }

            return results;
        }


        // intentionally public (secret menu item)
        public static PivotPointsResult GetPivotPoint(
            PivotPointType type, decimal open, decimal high, decimal low, decimal close)
        {
            PivotPointsResult wp = new PivotPointsResult();

            switch (type)
            {
                case PivotPointType.Standard:

                    wp.PP = (high + low + close) / 3;
                    wp.S1 = wp.PP * 2 - high;
                    wp.S2 = wp.PP - (high - low);
                    wp.R1 = wp.PP * 2 - low;
                    wp.R2 = wp.PP + (high - low);
                    break;

                case PivotPointType.Camarilla:

                    wp.PP = (high + low + close) / 3;
                    wp.S1 = close - 1.0833m * (high - low);
                    wp.S2 = close - 1.1666m * (high - low);
                    wp.S3 = close - 1.2500m * (high - low);
                    wp.S4 = close - 1.5000m * (high - low);
                    wp.R1 = close + 1.0833m * (high - low);
                    wp.R2 = close + 1.1666m * (high - low);
                    wp.R3 = close + 1.2500m * (high - low);
                    wp.R4 = close + 1.5000m * (high - low);
                    break;

                case PivotPointType.Demark:

                    decimal? x;

                    if (close < open)
                    {
                        x = high + 2 * low + close;
                    }
                    else if (close > open)
                    {
                        x = 2 * high + low + close;
                    }
                    else if (close == open)
                    {
                        x = high + low + 2 * close;
                    }
                    else
                    {
                        x = null;
                    }

                    wp.PP = x / 4;
                    wp.S1 = x / 2 - high;
                    wp.R1 = x / 2 - low;
                    break;

                case PivotPointType.Fibonacci:

                    wp.PP = (high + low + close) / 3;
                    wp.S1 = wp.PP - 0.382m * (high - low);
                    wp.S2 = wp.PP - 0.618m * (high - low);
                    wp.S3 = wp.PP - 1.000m * (high - low);
                    wp.R1 = wp.PP + 0.382m * (high - low);
                    wp.R2 = wp.PP + 0.618m * (high - low);
                    wp.R3 = wp.PP + 1.000m * (high - low);
                    break;

                case PivotPointType.Woodie:

                    wp.PP = (high + low + 2 * close) / 4;
                    wp.S1 = wp.PP * 2 - high;
                    wp.S2 = wp.PP - high + low;
                    wp.R1 = wp.PP * 2 - low;
                    wp.R2 = wp.PP + high - low;
                    break;

                default:
                    break;
            }

            return wp;
        }

        private static int GetWindowNumber(DateTime d, PeriodSize windowSize)
        {
            return windowSize switch
            {
                PeriodSize.Month => d.Month,
                PeriodSize.Week => englishCalendar.GetWeekOfYear(d, englishCalendarWeekRule, englishFirstDayOfWeek),
                PeriodSize.Day => d.Day,
                PeriodSize.Hour => d.Hour,
                _ => 0
            };
        }


        private static void ValidatePivotPoints<TQuote>(IEnumerable<TQuote> history, PeriodSize windowSize)
            where TQuote : IQuote
        {

            // count periods based on periodSize
            int qtyWindows = windowSize switch
            {
                PeriodSize.Month => history.Select(x => x.Date.Month).Distinct().Count(),

                PeriodSize.Week => history.Select(x => englishCalendar
                    .GetWeekOfYear(x.Date, englishCalendarWeekRule, englishFirstDayOfWeek))
                    .Distinct().Count(),

                PeriodSize.Day => history.Select(x => x.Date.Day).Distinct().Count(),

                PeriodSize.Hour => history.Select(x => x.Date.Hour).Distinct().Count(),

                _ => 0
            };

            // check history to ensure 2+ periods are present

            if (qtyWindows < 2)
            {
                string message = "Insufficient history provided for Pivot Points.  " +
                    string.Format(englishCulture,
                    "You provided {0} {1} windows of history when at least 2 are required.  "
                    + "This can be from either not enough history or insufficiently detailed Date values.",
                    qtyWindows, Enum.GetName(typeof(PeriodSize), windowSize));

                throw new BadHistoryException(nameof(history), message);
            }

        }
    }

}
