namespace Skender.Stock.Indicators;

// PIVOT POINTS (SERIES)

public static partial class Indicator
{
    private static List<PivotPointsResult> CalcPivotPoints<TQuote>(
        this List<TQuote> quotesList,
        PeriodSize windowSize,
        PivotPointType pointType)
        where TQuote : IQuote
    {
        // initialize
        int length = quotesList.Count;
        List<PivotPointsResult> results = new(length);

        WindowPoint windowPoint = new();

        if (length == 0)
        {
            return results;
        }

        TQuote h0 = quotesList[0];

        int windowId = GetWindowNumber(h0.Timestamp, windowSize);

        bool firstWindow = true;

        decimal windowHigh = h0.High;
        decimal windowLow = h0.Low;
        decimal windowOpen = h0.Open;
        decimal windowClose = h0.Close;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            TQuote q = quotesList[i];

            // new window evaluation
            int windowEval = GetWindowNumber(q.Timestamp, windowSize);

            if (windowEval != windowId)
            {
                windowId = windowEval;
                firstWindow = false;

                // set new levels
                if (pointType == PivotPointType.Woodie)
                {
                    windowOpen = q.Open;
                }

                windowPoint = GetPivotPoint(
                    pointType, windowOpen, windowHigh, windowLow, windowClose);

                // reset window min/max thresholds
                windowOpen = q.Open;
                windowHigh = q.High;
                windowLow = q.Low;
            }

            // add levels
            PivotPointsResult r
                = !firstWindow
                ? new() {

                    Timestamp = q.Timestamp,

                    // pivot point
                    PP = windowPoint.PP,

                    // support
                    S1 = windowPoint.S1,
                    S2 = windowPoint.S2,
                    S3 = windowPoint.S3,
                    S4 = windowPoint.S4,

                    // resistance
                    R1 = windowPoint.R1,
                    R2 = windowPoint.R2,
                    R3 = windowPoint.R3,
                    R4 = windowPoint.R4
                }
                : new PivotPointsResult {
                    Timestamp = q.Timestamp
                };

            results.Add(r);

            // capture window threholds (for next iteration)
            windowHigh = q.High > windowHigh ? q.High : windowHigh;
            windowLow = q.Low < windowLow ? q.Low : windowLow;
            windowClose = q.Close;
        }

        return results;
    }

    // internals
    private static WindowPoint GetPivotPointStandard(
        decimal high, decimal low, decimal close)
    {
        decimal pp = (high + low + close) / 3;

        return new() {
            PP = pp,
            S1 = pp * 2 - high,
            S2 = pp - (high - low),
            S3 = low - 2 * (high - pp),
            R1 = pp * 2 - low,
            R2 = pp + (high - low),
            R3 = high + 2 * (pp - low)
        };
    }

    private static WindowPoint GetPivotPointCamarilla(
        decimal high, decimal low, decimal close)
        => new() {
            PP = close,
            S1 = close - 1.1m / 12 * (high - low),
            S2 = close - 1.1m / 6 * (high - low),
            S3 = close - 1.1m / 4 * (high - low),
            S4 = close - 1.1m / 2 * (high - low),
            R1 = close + 1.1m / 12 * (high - low),
            R2 = close + 1.1m / 6 * (high - low),
            R3 = close + 1.1m / 4 * (high - low),
            R4 = close + 1.1m / 2 * (high - low)
        };

    internal static WindowPoint GetPivotPointDemark(
        decimal open, decimal high, decimal low, decimal close)
    {
        decimal x = close < open
            ? high + 2 * low + close
            : close > open
            ? 2 * high + low + close
            : high + low + 2 * close;

        return new() {
            PP = x / 4,
            S1 = x / 2 - high,
            R1 = x / 2 - low
        };
    }

    private static WindowPoint GetPivotPointFibonacci(
        decimal high, decimal low, decimal close)
    {
        decimal pp = (high + low + close) / 3;

        return new() {
            PP = pp,
            S1 = pp - 0.382m * (high - low),
            S2 = pp - 0.618m * (high - low),
            S3 = pp - 1.000m * (high - low),
            R1 = pp + 0.382m * (high - low),
            R2 = pp + 0.618m * (high - low),
            R3 = pp + 1.000m * (high - low)
        };
    }

    private static WindowPoint GetPivotPointWoodie(
        decimal currentOpen, decimal high, decimal low)
    {
        decimal pp = (high + low + 2 * currentOpen) / 4;

        return new() {
            PP = pp,
            S1 = pp * 2 - high,
            S2 = pp - high + low,
            S3 = low - 2 * (high - pp),
            R1 = pp * 2 - low,
            R2 = pp + high - low,
            R3 = high + 2 * (pp - low)
        };
    }

    // pivot type lookup
    private static WindowPoint GetPivotPoint(
        PivotPointType pointType, decimal open, decimal high, decimal low, decimal close)
        => pointType switch {

            PivotPointType.Standard => GetPivotPointStandard(high, low, close),
            PivotPointType.Camarilla => GetPivotPointCamarilla(high, low, close),
            PivotPointType.Demark => GetPivotPointDemark(open, high, low, close),
            PivotPointType.Fibonacci => GetPivotPointFibonacci(high, low, close),
            PivotPointType.Woodie => GetPivotPointWoodie(open, high, low),

            _ => throw new ArgumentOutOfRangeException(
                nameof(pointType), pointType, "Invalid pointType provided.")
        };

    // window size lookup
    private static int GetWindowNumber(DateTime d, PeriodSize windowSize)
        => windowSize switch {

            PeriodSize.Month => d.Month,

            PeriodSize.Week => EnglishCalendar.GetWeekOfYear(
                d, EnglishCalendarWeekRule, EnglishFirstDayOfWeek),

            PeriodSize.Day => d.Day,
            PeriodSize.OneHour => d.Hour,

            _ => throw new ArgumentOutOfRangeException(
                nameof(windowSize), windowSize,
                string.Format(
                    EnglishCulture,
                    "Pivot Points does not support PeriodSize of {0}.  " +
                    "See documentation for valid options.",
                    Enum.GetName(typeof(PeriodSize), windowSize)))
        };
}
