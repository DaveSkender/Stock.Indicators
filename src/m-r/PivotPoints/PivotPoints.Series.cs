namespace Skender.Stock.Indicators;

// PIVOT POINTS (SERIES)
public static partial class Indicator
{
    internal static List<PivotPointsResult> CalcPivotPoints<TQuote>(
        this List<TQuote> quotesList,
        PeriodSize windowSize,
        PivotPointType pointType)
        where TQuote : IQuote
    {
        // initialize
        int length = quotesList.Count;
        List<PivotPointsResult> results = new(length);
        PivotPointsResult? windowPoint = new();
        TQuote h0;

        if (length == 0)
        {
            return results;
        }
        else
        {
            h0 = quotesList[0];
        }

        int windowId = GetWindowNumber(h0.Date, windowSize);

        int windowEval;
        bool firstWindow = true;

        decimal windowHigh = h0.High;
        decimal windowLow = h0.Low;
        decimal windowOpen = h0.Open;
        decimal windowClose = h0.Close;

        // roll through quotes
        for (int i = 0; i < length; i++)
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
                r.PP = windowPoint?.PP;

                // support
                r.S1 = windowPoint?.S1;
                r.S2 = windowPoint?.S2;
                r.S3 = windowPoint?.S3;
                r.S4 = windowPoint?.S4;

                // resistance
                r.R1 = windowPoint?.R1;
                r.R2 = windowPoint?.R2;
                r.R3 = windowPoint?.R3;
                r.R4 = windowPoint?.R4;
            }

            results.Add(r);

            // capture window threholds (for next iteration)
            windowHigh = (q.High > windowHigh) ? q.High : windowHigh;
            windowLow = (q.Low < windowLow) ? q.Low : windowLow;
            windowClose = q.Close;
        }

        return results;
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
            S1 = (pp * 2) - high,
            S2 = pp - (high - low),
            S3 = low - (2 * (high - pp)),
            R1 = (pp * 2) - low,
            R2 = pp + (high - low),
            R3 = high + (2 * (pp - low))
        };
    }

    internal static TPivotPoint GetPivotPointCamarilla<TPivotPoint>(
        decimal high, decimal low, decimal close)
        where TPivotPoint : IPivotPoint, new()
        => new()
        {
            PP = close,
            S1 = close - (1.1m / 12 * (high - low)),
            S2 = close - (1.1m / 6 * (high - low)),
            S3 = close - (1.1m / 4 * (high - low)),
            S4 = close - (1.1m / 2 * (high - low)),
            R1 = close + (1.1m / 12 * (high - low)),
            R2 = close + (1.1m / 6 * (high - low)),
            R3 = close + (1.1m / 4 * (high - low)),
            R4 = close + (1.1m / 2 * (high - low))
        };

    internal static TPivotPoint GetPivotPointDemark<TPivotPoint>(
        decimal open, decimal high, decimal low, decimal close)
        where TPivotPoint : IPivotPoint, new()
    {
        decimal? x = null;

        if (close < open)
        {
            x = high + (2 * low) + close;
        }
        else if (close > open)
        {
            x = (2 * high) + low + close;
        }
        else if (close == open)
        {
            x = high + low + (2 * close);
        }

        return new TPivotPoint
        {
            PP = x / 4,
            S1 = (x / 2) - high,
            R1 = (x / 2) - low
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
            S1 = pp - (0.382m * (high - low)),
            S2 = pp - (0.618m * (high - low)),
            S3 = pp - (1.000m * (high - low)),
            R1 = pp + (0.382m * (high - low)),
            R2 = pp + (0.618m * (high - low)),
            R3 = pp + (1.000m * (high - low))
        };
    }

    internal static TPivotPoint GetPivotPointWoodie<TPivotPoint>(
        decimal currentOpen, decimal high, decimal low)
        where TPivotPoint : IPivotPoint, new()
    {
        decimal pp = (high + low + (2 * currentOpen)) / 4;

        return new TPivotPoint
        {
            PP = pp,
            S1 = (pp * 2) - high,
            S2 = pp - high + low,
            S3 = low - (2 * (high - pp)),
            R1 = (pp * 2) - low,
            R2 = pp + high - low,
            R3 = high + (2 * (pp - low)),
        };
    }

    // pivot type lookup
    internal static TPivotPoint? GetPivotPoint<TPivotPoint>(
        PivotPointType pointType, decimal open, decimal high, decimal low, decimal close)
        where TPivotPoint : IPivotPoint, new()
        => pointType switch
        {
            PivotPointType.Standard => GetPivotPointStandard<TPivotPoint>(high, low, close),
            PivotPointType.Camarilla => GetPivotPointCamarilla<TPivotPoint>(high, low, close),
            PivotPointType.Demark => GetPivotPointDemark<TPivotPoint>(open, high, low, close),
            PivotPointType.Fibonacci => GetPivotPointFibonacci<TPivotPoint>(high, low, close),
            PivotPointType.Woodie => GetPivotPointWoodie<TPivotPoint>(open, high, low),
            _ => throw new ArgumentOutOfRangeException(nameof(pointType), pointType, "Invalid pointType provided.")
        };

    // window size lookup
    private static int GetWindowNumber(DateTime d, PeriodSize windowSize)
        => windowSize switch
        {
            PeriodSize.Month => d.Month,
            PeriodSize.Week => EnglishCalendar.GetWeekOfYear(d, EnglishCalendarWeekRule, EnglishFirstDayOfWeek),
            PeriodSize.Day => d.Day,
            PeriodSize.OneHour => d.Hour,
            _ => throw new ArgumentOutOfRangeException(nameof(windowSize), windowSize,
                string.Format(
                    EnglishCulture,
                    "Pivot Points does not support PeriodSize of {0}.  See documentation for valid options.",
                    Enum.GetName(typeof(PeriodSize), windowSize)))
        };
}
