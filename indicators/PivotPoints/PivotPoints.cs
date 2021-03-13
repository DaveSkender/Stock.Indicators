using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // PIVOT POINTS
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<PivotPointsResult> GetPivotPoints<TQuote>(
            IEnumerable<TQuote> history,
            PeriodSize windowSize,
            PivotPointType pointType = PivotPointType.Standard)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidatePivotPoints(history, windowSize);

            // initialize
            List<PivotPointsResult> results = new(historyList.Count);
            PivotPointsResult windowPoint = new();

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

                PivotPointsResult r = new()
                {
                    Date = h.Date
                };

                // new window evaluation
                windowEval = GetWindowNumber(h.Date, windowSize);

                if (windowEval != windowId)
                {
                    windowId = windowEval;
                    firstWindow = false;

                    // set new levels
                    if (pointType == PivotPointType.Woodie)
                    {
                        windowOpen = h.Open;
                    }

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
                PivotPointType.Woodie => GetPivotPointWoodie(open, high, low),
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
                PP = close,
                S1 = close - (1.1m / 12) * (high - low),
                S2 = close - (1.1m / 6) * (high - low),
                S3 = close - (1.1m / 4) * (high - low),
                S4 = close - (1.1m / 2) * (high - low),
                R1 = close + (1.1m / 12) * (high - low),
                R2 = close + (1.1m / 6) * (high - low),
                R3 = close + (1.1m / 4) * (high - low),
                R4 = close + (1.1m / 2) * (high - low)
            };
        }

        public static PivotPointsResult GetPivotPointDemark(
            decimal open, decimal high, decimal low, decimal close)
        {
            decimal? x = null;

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
            decimal currentOpen, decimal high, decimal low)
        {
            decimal pp = (high + low + 2 * currentOpen) / 4;

            return new PivotPointsResult
            {
                PP = pp,
                S1 = pp * 2 - high,
                S2 = pp - high + low,
                S3 = low - 2 * (high - pp),
                R1 = pp * 2 - low,
                R2 = pp + high - low,
                R3 = high + 2 * (pp - low),
            };
        }


        private static int GetWindowNumber(DateTime d, PeriodSize windowSize)
        {
            return windowSize switch
            {
                PeriodSize.Month => d.Month,
                PeriodSize.Week => EnglishCalendar.GetWeekOfYear(d, EnglishCalendarWeekRule, EnglishFirstDayOfWeek),
                PeriodSize.Day => d.Day,
                PeriodSize.Hour => d.Hour,
                _ => 0
            };
        }


        private static void ValidatePivotPoints<TQuote>(
            IEnumerable<TQuote> history,
            PeriodSize windowSize)
            where TQuote : IQuote
        {

            // check parameter arguments
            int qtyWindows = 0;

            switch (windowSize)
            {
                case PeriodSize.Month:
                    qtyWindows = history.Select(x => x.Date.Month).Distinct().Count();
                    break;

                case PeriodSize.Week:
                    qtyWindows = history.Select(x => EnglishCalendar
                    .GetWeekOfYear(x.Date, EnglishCalendarWeekRule, EnglishFirstDayOfWeek))
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

            // check history
            if (qtyWindows < 2)
            {
                string message = "Insufficient history provided for Pivot Points.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} {1} windows of history when at least 2 are required.  "
                    + "This can be from either not enough history or insufficiently detailed Date values.",
                    qtyWindows, Enum.GetName(typeof(PeriodSize), windowSize));

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
