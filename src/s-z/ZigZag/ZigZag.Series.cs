namespace Skender.Stock.Indicators;

// ZIG ZAG (SERIES)
public static partial class Indicator
{
    internal static List<ZigZagResult> CalcZigZag<TQuote>(
        this List<TQuote> quotesList,
        EndType endType = EndType.Close,
        decimal percentChange = 5)
        where TQuote : IQuote
    {
        // check parameter arguments
        ValidateZigZag(percentChange);

        // initialize
        int length = quotesList.Count;
        List<ZigZagResult> results = new(length);
        TQuote q0;

        if (length == 0)
        {
            return results;
        }
        else
        {
            q0 = quotesList[0];
        }

        ZigZagEval eval = GetZigZagEval(endType, 1, q0);
        decimal changeThreshold = percentChange / 100m;

        ZigZagPoint lastPoint = new()
        {
            Index = eval.Index,
            Value = q0.Close,
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

        int finalPointIndex = length;

        // roll through quotes, to find initial trend
        for (int i = 0; i < length; i++)
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
        ZigZagResult firstResult = new(q0.Date);
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
        EndType endType,
        decimal changeThreshold,
        ZigZagPoint lastPoint)
        where TQuote : IQuote
    {
        // initialize
        bool trendUp = lastPoint.PointType == "L";

        // candidate
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
            decimal? change;

            ZigZagEval eval = GetZigZagEval(endType, index, q);

            // reset extreme point
            if (trendUp)
            {
                if (eval.High >= extremePoint.Value)
                {
                    extremePoint.Index = eval.Index;
                    extremePoint.Value = eval.High;
                    change = 0;
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
                    change = 0;
                }
                else
                {
                    change = (extremePoint.Value == 0) ? null
                        : (eval.High - extremePoint.Value) / extremePoint.Value;
                }
            }

            // return extreme point when reversion threshold met
            if (change >= changeThreshold)
            {
                return extremePoint;
            }

            // add last unconfirmed end point
            if (index == quotesList.Count)
            {
                extremePoint.Index = index;
                extremePoint.Value = trendUp ? eval.High : eval.Low;
                extremePoint.PointType = null;
            }
        }

        return extremePoint;
    }

    private static void DrawZigZagLine<TQuote>(List<ZigZagResult> results, List<TQuote> quotesList,
        ZigZagPoint lastPoint, ZigZagPoint nextPoint)
        where TQuote : IQuote
    {
        if (nextPoint.Index != lastPoint.Index)
        {
            decimal? increment = (nextPoint.Value - lastPoint.Value) / (nextPoint.Index - lastPoint.Index);

            // add new line segment
            for (int i = lastPoint.Index; i < nextPoint.Index; i++)
            {
                TQuote q = quotesList[i];
                int index = i + 1;

                ZigZagResult result = new(q.Date)
                {
                    ZigZag = (lastPoint.Index != 1 || index == nextPoint.Index) ?
                        lastPoint.Value + (increment * (index - lastPoint.Index)) : null,
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

    private static void DrawRetraceLine(
        List<ZigZagResult> results,
        string lastDirection,
        ZigZagPoint lastLowPoint,
        ZigZagPoint lastHighPoint,
        ZigZagPoint nextPoint)
    {
        ZigZagPoint priorPoint = new();

        // handle type and reset last point
        if (lastDirection == "L")
        {
            priorPoint.Index = lastHighPoint.Index;
            priorPoint.Value = lastHighPoint.Value;

            lastHighPoint.Index = nextPoint.Index;
            lastHighPoint.Value = nextPoint.Value;
        }

        // low line
        else if (lastDirection == "H")
        {
            priorPoint.Index = lastLowPoint.Index;
            priorPoint.Value = lastLowPoint.Value;

            lastLowPoint.Index = nextPoint.Index;
            lastLowPoint.Value = nextPoint.Value;
        }

        // nothing to draw cases
        if (
            lastDirection == "U" // first line skipped, single line
         || priorPoint.Index == 1 // first line skipped, normal case
         || nextPoint.Index == priorPoint.Index) // no span
        {
            return;
        }

        // narrow to period
        decimal? increment = (nextPoint.Value - priorPoint.Value) / (nextPoint.Index - priorPoint.Index);

        // add new line segment
        for (int i = priorPoint.Index - 1; i < nextPoint.Index; i++)
        {
            ZigZagResult r = results[i];
            int index = i + 1;

            // high line
            if (lastDirection == "L")
            {
                r.RetraceHigh = priorPoint.Value + (increment * (index - priorPoint.Index));
            }

            // low line
            else if (lastDirection == "H")
            {
                r.RetraceLow = priorPoint.Value + (increment * (index - priorPoint.Index));
            }
        }
    }

    private static ZigZagEval GetZigZagEval<TQuote>(
        EndType endType,
        int index,
        TQuote q)
        where TQuote : IQuote
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
    private static void ValidateZigZag(
        decimal percentChange)
    {
        // check parameter arguments
        if (percentChange <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(percentChange), percentChange,
                "Percent change must be greater than 0 for ZIGZAG.");
        }
    }
}
