using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // ZIG ZAG
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<ZigZagResult> GetZigZag<TQuote>(
            this IEnumerable<TQuote> quotes,
            EndType endType = EndType.Close,
            decimal percentChange = 5)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateZigZag(quotes, percentChange);

            // initialize
            List<ZigZagResult> results = new(quotesList.Count);
            decimal changeThreshold = percentChange / 100m;
            TQuote firstQuote = quotesList[0];
            ZigZagEval eval = GetZigZagEval(endType, 1, firstQuote);

            ZigZagPoint lastPoint = new()
            {
                Index = eval.Index,
                Value = firstQuote.Close,
                PointType = "U"
            };

            ZigZagPoint lastHighPoint = new()
            {
                Index = eval.Index,
                Value = eval.High,
                PointType = "H"
            };

            ZigZagPoint lastLowPoint = new()
            {
                Index = eval.Index,
                Value = eval.Low,
                PointType = "L"
            };

            int finalPointIndex = quotesList.Count;

            // roll through quotes, to find initial trend
            for (int i = 0; i < quotesList.Count; i++)
            {
                TQuote q = quotesList[i];
                int index = i + 1;

                eval = GetZigZagEval(endType, index, q);

                decimal? changeUp = (lastLowPoint.Value == 0) ? null
                    : (eval.High - lastLowPoint.Value) / lastLowPoint.Value;

                decimal? changeDn = (lastHighPoint.Value == 0) ? null
                    : (lastHighPoint.Value - eval.Low) / lastHighPoint.Value;

                if (changeUp >= changeThreshold && changeUp > changeDn)
                {
                    lastPoint.Index = lastLowPoint.Index;
                    lastPoint.Value = lastLowPoint.Value;
                    lastPoint.PointType = lastLowPoint.PointType;
                    break;
                }

                if (changeDn >= changeThreshold && changeDn > changeUp)
                {
                    lastPoint.Index = lastHighPoint.Index;
                    lastPoint.Value = lastHighPoint.Value;
                    lastPoint.PointType = lastHighPoint.PointType;
                    break;
                }
            }

            // add first point to results
            ZigZagResult firstResult = new()
            {
                Date = firstQuote.Date
            };
            results.Add(firstResult);

            // find and draw lines
            while (lastPoint.Index < finalPointIndex)
            {
                ZigZagPoint nextPoint = EvaluateNextPoint(quotesList, endType, changeThreshold, lastPoint);
                string lastDirection = lastPoint.PointType;

                // draw line (and reset last point)
                DrawZigZagLine(results, quotesList, lastPoint, nextPoint);

                // draw retrace line (and reset last high/low point)
                DrawRetraceLine(results, lastDirection, lastLowPoint, lastHighPoint, nextPoint);
            }

            return results;
        }


        // internals
        private static ZigZagPoint EvaluateNextPoint<TQuote>(
            List<TQuote> quotesList,
            EndType endType, decimal changeThreshold, ZigZagPoint lastPoint) where TQuote : IQuote
        {
            // initialize
            bool trendUp = (lastPoint.PointType == "L");
            decimal? change = 0;

            ZigZagPoint extremePoint = new()
            {
                Index = lastPoint.Index,
                Value = lastPoint.Value,
                PointType = trendUp ? "H" : "L"
            };

            // find extreme point before reversal point
            for (int i = lastPoint.Index; i < quotesList.Count; i++)
            {
                TQuote q = quotesList[i];
                int index = i + 1;

                ZigZagEval eval = GetZigZagEval(endType, index, q);

                // reset extreme point
                if (trendUp)
                {
                    if (eval.High >= extremePoint.Value)
                    {
                        extremePoint.Index = eval.Index;
                        extremePoint.Value = eval.High;
                    }
                    else
                    {
                        change = (extremePoint.Value == 0) ? null
                            : (extremePoint.Value - eval.Low) / extremePoint.Value;
                    }
                }
                else
                {
                    if (eval.Low <= extremePoint.Value)
                    {
                        extremePoint.Index = eval.Index;
                        extremePoint.Value = eval.Low;
                    }
                    else
                    {
                        change = (extremePoint.Value == 0) ? null
                            : (eval.High - extremePoint.Value) / extremePoint.Value;
                    }
                }

                // return extreme point when deviation threshold met
                if (change >= changeThreshold)
                {
                    return extremePoint;
                }
            }

            // handle last unconfirmed point
            int finalPointIndex = quotesList.Count;
            if (extremePoint.Index == finalPointIndex && change < changeThreshold)
            {
                extremePoint.PointType = null;
            }

            return extremePoint;
        }


        private static void DrawZigZagLine<TQuote>(List<ZigZagResult> results, List<TQuote> quotesList,
            ZigZagPoint lastPoint, ZigZagPoint nextPoint) where TQuote : IQuote
        {

            if (nextPoint.Index != lastPoint.Index)
            {
                decimal increment = (nextPoint.Value - lastPoint.Value) / (nextPoint.Index - lastPoint.Index);

                // add new line segment
                for (int i = lastPoint.Index; i < nextPoint.Index; i++)
                {
                    TQuote q = quotesList[i];
                    int index = i + 1;

                    ZigZagResult result = new()
                    {
                        Date = q.Date,
                        ZigZag = (lastPoint.Index != 1 || index == nextPoint.Index) ?
                            lastPoint.Value + increment * (index - lastPoint.Index) : null,
                        PointType = (index == nextPoint.Index) ? nextPoint.PointType : null
                    };

                    results.Add(result);
                }
            }

            // reset lastpoint
            lastPoint.Index = nextPoint.Index;
            lastPoint.Value = nextPoint.Value;
            lastPoint.PointType = nextPoint.PointType;
        }


        private static void DrawRetraceLine(List<ZigZagResult> results, string lastDirection,
            ZigZagPoint lastLowPoint, ZigZagPoint lastHighPoint, ZigZagPoint nextPoint)
        {
            bool isHighLine = (lastDirection == "L");
            ZigZagPoint priorPoint = new();

            // handle type and reset last point
            if (isHighLine)
            {
                priorPoint.Index = lastHighPoint.Index;
                priorPoint.Value = lastHighPoint.Value;

                lastHighPoint.Index = nextPoint.Index;
                lastHighPoint.Value = nextPoint.Value;
            }
            else
            {
                priorPoint.Index = lastLowPoint.Index;
                priorPoint.Value = lastLowPoint.Value;

                lastLowPoint.Index = nextPoint.Index;
                lastLowPoint.Value = nextPoint.Value;
            }

            // nothing to do if first line or no-span case
            if (priorPoint.Index == 1 || nextPoint.Index == priorPoint.Index)
            {
                return;
            }

            // narrow to period
            decimal increment = (nextPoint.Value - priorPoint.Value) / (nextPoint.Index - priorPoint.Index);

            // add new line segment
            //foreach (ZigZagResult r in period)
            for (int i = priorPoint.Index - 1; i < nextPoint.Index; i++)
            {
                ZigZagResult r = results[i];
                int index = i + 1;

                if (isHighLine)
                {
                    r.RetraceHigh = priorPoint.Value + increment * (index - priorPoint.Index);
                }
                else
                {
                    r.RetraceLow = priorPoint.Value + increment * (index - priorPoint.Index);
                }
            }
        }


        private static ZigZagEval GetZigZagEval<TQuote>(EndType endType, int index, TQuote q) where TQuote : IQuote
        {
            ZigZagEval eval = new()
            {
                Index = index
            };

            // consider `type`
            switch (endType)
            {
                case EndType.Close:

                    eval.Low = q.Close;
                    eval.High = q.Close;
                    break;

                case EndType.HighLow:

                    eval.Low = q.Low;
                    eval.High = q.High;
                    break;

                default:
                    break;
            }

            return eval;
        }


        // parameter validation
        private static void ValidateZigZag<TQuote>(
            IEnumerable<TQuote> quotes,
            decimal percentChange)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (percentChange <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(percentChange), percentChange,
                    "Percent change must be greater than 0 for ZIGZAG.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = 2;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for ZIGZAG.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
