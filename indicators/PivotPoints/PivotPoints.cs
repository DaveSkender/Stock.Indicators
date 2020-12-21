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


        internal static PivotPointsResult GetPivotPoint(
            PivotPointType type, decimal open, decimal high, decimal low, decimal close)
        {
            return type switch
            {
                PivotPointType.Standard => GetPivotPointStandard(high, low, close),
                PivotPointType.Camarilla => GetPivotPointCamarilla(high, low, close),
                PivotPointType.Demark => GetPivotPointDemark(open, high, low, close),
                PivotPointType.Fibonacci => GetPivotPointFibonacci(high, low, close),
                PivotPointType.Woodie => GetPivotPointWoodie(high, low, close),
                _ => null
            };
        }

        public static PivotPointsResult GetPivotPointStandard(
            decimal high, decimal low, decimal close)
        {
            decimal pp = (high + low + close) / 3;

            return new PivotPointsResult
            {
                PP = pp,
                S1 = pp * 2 - high,
                S2 = pp - (high - low),
                R1 = pp * 2 - low,
                R2 = pp + (high - low)
            };
        }

        public static PivotPointsResult GetPivotPointCamarilla(
            decimal high, decimal low, decimal close)
        {
            return new PivotPointsResult
            {
                PP = (high + low + close) / 3,
                S1 = close - 1.0833m * (high - low),
                S2 = close - 1.1666m * (high - low),
                S3 = close - 1.2500m * (high - low),
                S4 = close - 1.5000m * (high - low),
                R1 = close + 1.0833m * (high - low),
                R2 = close + 1.1666m * (high - low),
                R3 = close + 1.2500m * (high - low),
                R4 = close + 1.5000m * (high - low)
            };
        }

        public static PivotPointsResult GetPivotPointDemark(
            decimal open, decimal high, decimal low, decimal close)
        {
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

            return new PivotPointsResult
            {
                PP = x / 4,
                S1 = x / 2 - high,
                R1 = x / 2 - low
            };
        }

        public static PivotPointsResult GetPivotPointFibonacci(
            decimal high, decimal low, decimal close)
        {
            decimal pp = (high + low + close) / 3;

            return new PivotPointsResult
            {
                PP = pp,
                S1 = pp - 0.382m * (high - low),
                S2 = pp - 0.618m * (high - low),
                S3 = pp - 1.000m * (high - low),
                R1 = pp + 0.382m * (high - low),
                R2 = pp + 0.618m * (high - low),
                R3 = pp + 1.000m * (high - low)
            };
        }

        public static PivotPointsResult GetPivotPointWoodie(
            decimal high, decimal low, decimal close)
        {
            decimal pp = (high + low + 2 * close) / 4;

            return new PivotPointsResult
            {
                PP = pp,
                S1 = pp * 2 - high,
                S2 = pp - high + low,
                R1 = pp * 2 - low,
                R2 = pp + high - low
            };
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


        private static void ValidatePivotPoints<TQuote>(
            IEnumerable<TQuote> history, PeriodSize windowSize)
            where TQuote : IQuote
        {
            //Console.WriteLine(pointType);

            //// check parameters
            //PeriodSize[] sizes = {
            //    PeriodSize.Hour,
            //    PeriodSize.Day,
            //    PeriodSize.Week,
            //    PeriodSize.Month
            //};

            //if (!sizes.Contains(windowSize))
            //{
            //    throw new ArgumentOutOfRangeException(nameof(windowSize), windowSize,
            //        "Window Size must be Hour, Day, Week, or Month for Pivot Points.");
            //}

            //PivotPointType[] points = {
            //    PivotPointType.Standard,
            //    PivotPointType.Camarilla,
            //    PivotPointType.Demark,
            //    PivotPointType.Fibonacci,
            //    PivotPointType.Woodie
            //};

            //if (!points.Contains(pointType))
            //{
            //    {
            //        throw new ArgumentOutOfRangeException(nameof(windowSize), windowSize,
            //            "Point Type must be Standard, Camarilla, Demark, Fibonacci, or Woodie for Pivot Points.");
            //    }
            //}

            // count periods based on periodSize
            int qtyWindows = 0;

            switch (windowSize)
            {
                case PeriodSize.Month:
                    qtyWindows = history.Select(x => x.Date.Month).Distinct().Count();
                    break;

                case PeriodSize.Week:
                    qtyWindows = history.Select(x => englishCalendar
                    .GetWeekOfYear(x.Date, englishCalendarWeekRule, englishFirstDayOfWeek))
                    .Distinct().Count();
                    break;

                case PeriodSize.Day:
                    qtyWindows = history.Select(x => x.Date.Day).Distinct().Count();
                    break;

                case PeriodSize.Hour:
                    qtyWindows = history.Select(x => x.Date.Hour).Distinct().Count();
                    break;

                default:
                    break;
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
