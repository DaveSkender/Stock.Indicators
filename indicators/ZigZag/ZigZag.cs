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
            IEnumerable<TQuote> history,
            ZigZagType type = ZigZagType.Close,
            decimal percentChange = 5)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateZigZag(history, percentChange);

            // initialize
            List<ZigZagResult> results = new(historyList.Count);
            decimal changeThreshold = percentChange / 100m;
            TQuote firstQuote = historyList[0];
            ZigZagEval eval = GetZigZagEval(type, 1, firstQuote);

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

            int finalPointIndex = historyList.Count;

            // roll through history, to find initial trend
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                eval = GetZigZagEval(type, index, h);

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
                ZigZagPoint nextPoint = EvaluateNextPoint(historyList, type, changeThreshold, lastPoint);
                string lastDirection = lastPoint.PointType;

                // draw line (and reset last point)
                DrawZigZagLine(results, historyList, lastPoint, nextPoint);

                // draw retrace line (and reset last high/low point)
                DrawRetraceLine(results, lastDirection, lastLowPoint, lastHighPoint, nextPoint);
            }

            return results;
        }


        private static ZigZagPoint EvaluateNextPoint<TQuote>(
            List<TQuote> historyList,
            ZigZagType type, decimal changeThreshold, ZigZagPoint lastPoint) where TQuote : IQuote
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
            for (int i = lastPoint.Index; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                ZigZagEval eval = GetZigZagEval(type, index, h);

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
            int finalPointIndex = historyList.Count;
            if (extremePoint.Index == finalPointIndex && change < changeThreshold)
            {
                extremePoint.PointType = null;
            }

            return extremePoint;
        }


        private static void DrawZigZagLine<TQuote>(List<ZigZagResult> results, List<TQuote> historyList,
            ZigZagPoint lastPoint, ZigZagPoint nextPoint) where TQuote : IQuote
        {

            if (nextPoint.Index != lastPoint.Index)
            {
                decimal increment = (nextPoint.Value - lastPoint.Value) / (nextPoint.Index - lastPoint.Index);

                // add new line segment
                for (int i = lastPoint.Index; i < nextPoint.Index; i++)
                {
                    TQuote h = historyList[i];
                    int index = i + 1;

                    ZigZagResult result = new()
                    {
                        Date = h.Date,
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

            // nothing to do if first line
            if (priorPoint.Index == 1)
            {
                return;
            }

            // handle error case
            if (nextPoint.Index == priorPoint.Index)
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


        private static ZigZagEval GetZigZagEval<TQuote>(ZigZagType type, int index, TQuote q) where TQuote : IQuote
        {
            ZigZagEval eval = new()
            {
                Index = index
            };

            // consider `type`
            switch (type)
            {
                case ZigZagType.Close:

                    eval.Low = q.Close;
                    eval.High = q.Close;
                    break;

                case ZigZagType.HighLow:

                    eval.Low = q.Low;
                    eval.High = q.High;
                    break;

                default:
                    break;
            }

            return eval;
        }


        private static void ValidateZigZag<TQuote>(
            IEnumerable<TQuote> history,
            decimal percentChange)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (percentChange <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(percentChange), percentChange,
                    "Percent change must be greater than 0 for ZIGZAG.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = 2;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for ZIGZAG.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
