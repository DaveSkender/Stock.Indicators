using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // ZIG ZAG
        public static IEnumerable<ZigZagResult> GetZigZag(
            IEnumerable<Quote> history, ZigZagType type = ZigZagType.Close, decimal percentChange = 5)
        {

            // clean quotes
            history = Cleaners.PrepareHistory(history);

            // check parameters
            ValidateZigZag(history, percentChange);

            // initialize
            List<ZigZagResult> results = new List<ZigZagResult>();
            decimal changeThreshold = percentChange / 100m;
            Quote firstQuote = history.First();
            ZigZagEval eval = GetZigZagEval(type, firstQuote);

            ZigZagPoint lastPoint = new ZigZagPoint
            {
                Index = eval.Index,
                Value = firstQuote.Close,
                PointType = "U"
            };

            ZigZagPoint lastHighPoint = new ZigZagPoint
            {
                Index = eval.Index,
                Value = eval.High,
                PointType = "H"
            };

            ZigZagPoint lastLowPoint = new ZigZagPoint
            {
                Index = eval.Index,
                Value = eval.Low,
                PointType = "L"
            };

            int finalPointIndex = history.Select(x => (int)x.Index).Max();

            // roll through history until to find initial trend
            foreach (Quote h in history)
            {
                eval = GetZigZagEval(type, h);
                decimal changeUp = (eval.High - lastLowPoint.Value) / lastLowPoint.Value;
                decimal changeDn = (lastHighPoint.Value - eval.Low) / lastHighPoint.Value;

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
            ZigZagResult firstResult = new ZigZagResult
            {
                Index = (int)firstQuote.Index,
                Date = firstQuote.Date,
                ZigZag = lastPoint.Value
            };
            results.Add(firstResult);

            // find and draw lines
            while (lastPoint.Index < finalPointIndex)
            {
                ZigZagPoint nextPoint = EvaluateNextPoint(history, type, changeThreshold, lastPoint);

                // draw line (and reset last point)
                lastPoint = DrawZigZagLine(results, history, lastPoint, nextPoint);

                // draw retrace line (and reset last high/low point)
                //DrawRetraceLine(results, history, lastLowPoint, lastHighPoint, nextPoint);

                //Console.WriteLine("last point: {0}", lastPoint.Index);
            }

            return results;
        }


        private static ZigZagPoint EvaluateNextPoint(IEnumerable<Quote> history,
            ZigZagType type, decimal changeThreshold, ZigZagPoint lastPoint)
        {
            // initialize 
            bool trendUp = (lastPoint.PointType == "L");
            decimal change = 0;
            ZigZagEval eval = new ZigZagEval();

            ZigZagPoint extremePoint = new ZigZagPoint
            {
                Index = lastPoint.Index,
                Value = lastPoint.Value,
                PointType = trendUp ? "H" : "L"
            };

            IEnumerable<Quote> period = history
                .Where(x => x.Index > lastPoint.Index);

            // find extreme point before reversal point
            foreach (Quote h in period)
            {
                eval = GetZigZagEval(type, h);

                // reset extreme point
                switch (trendUp)
                {
                    case true:

                        if (eval.High >= extremePoint.Value)
                        {
                            extremePoint.Index = eval.Index;
                            extremePoint.Value = eval.High;
                        }
                        else
                        {
                            change = (extremePoint.Value - eval.Low) / extremePoint.Value;
                        }

                        break;

                    case false:

                        if (eval.Low <= extremePoint.Value)
                        {
                            extremePoint.Index = eval.Index;
                            extremePoint.Value = eval.Low;
                        }
                        else
                        {
                            change = (eval.High - extremePoint.Value) / extremePoint.Value;
                        }

                        break;
                }

                // return extreme point when deviation threshold met
                if (change >= changeThreshold)
                {
                    return extremePoint;
                }
            }

            // handle last unconfirmed point
            int finalPointIndex = history.Select(x => (int)x.Index).Max();
            if (extremePoint.Index == finalPointIndex && change < changeThreshold)
            {
                extremePoint.PointType = null;
            }

            return extremePoint;
        }


        private static ZigZagPoint DrawZigZagLine(List<ZigZagResult> results, IEnumerable<Quote> history,
            ZigZagPoint lastPoint, ZigZagPoint nextPoint)
        {
            // initialize
            List<ZigZagResult> newResults = new List<ZigZagResult>();

            IEnumerable<Quote> period = history
                .Where(x => x.Index > lastPoint.Index && x.Index <= nextPoint.Index);

            decimal increment = (nextPoint.Value - lastPoint.Value) / (nextPoint.Index - lastPoint.Index);

            // add new line segment
            foreach (Quote h in period)
            {

                ZigZagResult result = new ZigZagResult
                {
                    Index = (int)h.Index,
                    Date = h.Date,
                    ZigZag = (lastPoint.Index != 1) ? lastPoint.Value + increment * (h.Index - lastPoint.Index) : null,
                    PointType = ((int)h.Index == nextPoint.Index) ? nextPoint.PointType : null
                };

                results.Add(result);
            }

            // reset lastpoint
            lastPoint.Index = nextPoint.Index;
            lastPoint.Value = nextPoint.Value;
            lastPoint.PointType = nextPoint.PointType;

            return lastPoint;
        }


        private static ZigZagEval GetZigZagEval(ZigZagType type, Quote q)
        {
            ZigZagEval eval = new ZigZagEval()
            {
                Index = (int)q.Index
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
            }

            return eval;
        }


        private static void ValidateZigZag(IEnumerable<Quote> history, decimal percentChange)
        {

            // check parameters
            if (percentChange <= 0)
            {
                throw new BadParameterException("Percent change must be greater than 0 for ZIGZAG.");
            }

            // check histor
            int qtyHistory = history.Count();
            int minHistory = 2;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for ZIGZAG.  " +
                        string.Format(cultureProvider,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistory, minHistory));
            }

        }
    }

}
