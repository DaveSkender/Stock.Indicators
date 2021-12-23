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
            this IEnumerable<TQuote> quotes,
            PeriodSize windowSize,
            PivotPointType pointType = PivotPointType.Standard)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.SortToList();

            // check parameter arguments
            ValidatePivotPoints(quotes, windowSize);

            // initialize
            List<PivotPointsResult> results = new(quotesList.Count);
            PivotPointsResult windowPoint = new();

            TQuote h0 = quotesList[0];
            int windowId = GetWindowNumber(h0.Date, windowSize);
            int windowEval;
            bool firstWindow = true;

            decimal windowHigh = h0.High;
            decimal windowLow = h0.Low;
            decimal windowOpen = h0.Open;
            decimal windowClose = h0.Close;

            // roll through quotes
            for (int i = 0; i < quotesList.Count; i++)
            {
                TQuote q = quotesList[i];

                PivotPointsResult r = new()
                {
                    Date = q.Date
                };

                // new window evaluation
                windowEval = GetWindowNumber(q.Date, windowSize);

                if (windowEval != windowId)
                {
                    windowId = windowEval;
                    firstWindow = false;

                    // set new levels
                    if (pointType == PivotPointType.Woodie)
                    {
                        windowOpen = q.Open;
                    }

                    windowPoint = GetPivotPoint<PivotPointsResult>(
                        pointType, windowOpen, windowHigh, windowLow, windowClose);

                    // reset window min/max thresholds
                    windowOpen = q.Open;
                    windowHigh = q.High;
                    windowLow = q.Low;
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
                windowHigh = (q.High > windowHigh) ? q.High : windowHigh;
                windowLow = (q.Low < windowLow) ? q.Low : windowLow;
                windowClose = q.Close;
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<PivotPointsResult> RemoveWarmupPeriods(
            this IEnumerable<PivotPointsResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.PP != null);

            return results.Remove(removePeriods);
        }


        // internals
        internal static TPivotPoint GetPivotPointStandard<TPivotPoint>(
            decimal high, decimal low, decimal close)
            where TPivotPoint : IPivotPoint, new()
        {
            decimal pp = (high + low + close) / 3;

            return new TPivotPoint
            {
                PP = pp,
                S1 = pp * 2 - high,
                S2 = pp - (high - low),
                S3 = low - 2 * (high - pp),
                R1 = pp * 2 - low,
                R2 = pp + (high - low),
                R3 = high + 2 * (pp - low)
            };
        }

        internal static TPivotPoint GetPivotPointCamarilla<TPivotPoint>(
            decimal high, decimal low, decimal close)
            where TPivotPoint : IPivotPoint, new()
        {
            return new TPivotPoint
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

        internal static TPivotPoint GetPivotPointDemark<TPivotPoint>(
            decimal open, decimal high, decimal low, decimal close)
            where TPivotPoint : IPivotPoint, new()
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

            return new TPivotPoint
            {
                PP = x / 4,
                S1 = x / 2 - high,
                R1 = x / 2 - low
            };
        }

        internal static TPivotPoint GetPivotPointFibonacci<TPivotPoint>(
            decimal high, decimal low, decimal close)
            where TPivotPoint : IPivotPoint, new()
        {
            decimal pp = (high + low + close) / 3;

            return new TPivotPoint
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

        internal static TPivotPoint GetPivotPointWoodie<TPivotPoint>(
            decimal currentOpen, decimal high, decimal low)
            where TPivotPoint : IPivotPoint, new()
        {
            decimal pp = (high + low + 2 * currentOpen) / 4;

            return new TPivotPoint
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


        // pivot type lookup
        internal static TPivotPoint GetPivotPoint<TPivotPoint>(
            PivotPointType pointType, decimal open, decimal high, decimal low, decimal close)
            where TPivotPoint : IPivotPoint, new()
        {
            return pointType switch
            {
                PivotPointType.Standard => GetPivotPointStandard<TPivotPoint>(high, low, close),
                PivotPointType.Camarilla => GetPivotPointCamarilla<TPivotPoint>(high, low, close),
                PivotPointType.Demark => GetPivotPointDemark<TPivotPoint>(open, high, low, close),
                PivotPointType.Fibonacci => GetPivotPointFibonacci<TPivotPoint>(high, low, close),
                PivotPointType.Woodie => GetPivotPointWoodie<TPivotPoint>(open, high, low),
                _ => default
            };
        }

        // window size lookup
        private static int GetWindowNumber(DateTime d, PeriodSize windowSize)
        {
            return windowSize switch
            {
                PeriodSize.Month => d.Month,
                PeriodSize.Week => EnglishCalendar.GetWeekOfYear(d, EnglishCalendarWeekRule, EnglishFirstDayOfWeek),
                PeriodSize.Day => d.Day,
                PeriodSize.OneHour => d.Hour,
                _ => 0
            };
        }

        // parameter validation
        private static void ValidatePivotPoints<TQuote>(
            IEnumerable<TQuote> quotes,
            PeriodSize windowSize)
            where TQuote : IQuote
        {

            // check parameter arguments
            int qtyWindows = windowSize switch
            {
                PeriodSize.Month => quotes
                    .Select(x => x.Date.Month).Distinct().Count(),

                PeriodSize.Week => quotes
                    .Select(x => EnglishCalendar
                    .GetWeekOfYear(x.Date, EnglishCalendarWeekRule, EnglishFirstDayOfWeek))
                    .Distinct().Count(),

                PeriodSize.Day => quotes
                    .Select(x => x.Date.Day).Distinct().Count(),

                PeriodSize.OneHour => quotes
                .Select(x => x.Date.Hour).Distinct().Count(),

                _ => 0
            };

            // check quotes
            if (qtyWindows < 2)
            {
                string message = "Insufficient quotes provided for Pivot Points.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} {1} windows of quotes when at least 2 are required.  "
                    + "This can be from either not enough quotes or insufficiently detailed Date values.",
                    qtyWindows, Enum.GetName(typeof(PeriodSize), windowSize));

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
