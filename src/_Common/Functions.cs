using System;

namespace Skender.Stock.Indicators
{
    internal static class Functions
    {
        // STANDARD DEVIATION
        internal static double StdDev(double[] values)
        {
            // ref: https://stackoverflow.com/questions/2253874/standard-deviation-in-linq
            // and then modified to an iterative model without LINQ, for performance improvement

            double sd = 0;
            int n = values.Length;
            if (n > 1)
            {
                double sum = 0;
                for (int i = 0; i < n; i++)
                {
                    sum += values[i];
                }

                double avg = sum / n;

                double sumSq = 0;
                for (int i = 0; i < n; i++)
                {
                    double d = values[i];
                    sumSq += (d - avg) * (d - avg);
                }

                sd = Math.Sqrt(sumSq / n);
            }
            return sd;
        }

        // SLOPE of BEST FIT LINE
        internal static double Slope(double[] x, double[] y)
        {
            // TODO: add better error handling for mismatch size arrays

            int size = Math.Min(x.Length, y.Length);

            // get averages for period
            double sumX = 0;
            double sumY = 0;

            for (int i = 0; i < size; i++)
            {
                sumX += x[i];
                sumY += y[i];
            }

            double avgX = sumX / size;
            double avgY = sumY / size;

            // least squares method
            double sumSqX = 0;
            double sumSqY = 0;
            double sumSqXY = 0;

            for (int i = 0; i < size; i++)
            {
                double devX = (x[i] - avgX);
                double devY = (y[i] - avgY);

                sumSqX += devX * devX;
                sumSqY += devY * devY;
                sumSqXY += devX * devY;
            }

            double slope = sumSqXY / sumSqX;

            return slope;
        }

        // DATE ROUNDING
        internal static DateTime RoundDown(this DateTime dateTime, TimeSpan interval)
        {
            return interval == TimeSpan.Zero ?

                  dateTime

                : dateTime
                    .AddTicks(-(dateTime.Ticks % interval.Ticks));
        }

        // PERIOD-SIZE to TIMESPAN CONVERSION
        internal static TimeSpan ToTimeSpan(this PeriodSize periodSize)
        {
            return periodSize switch
            {
                PeriodSize.OneMinute => TimeSpan.FromMinutes(1),
                PeriodSize.TwoMinutes => TimeSpan.FromMinutes(2),
                PeriodSize.ThreeMinutes => TimeSpan.FromMinutes(3),
                PeriodSize.FiveMinutes => TimeSpan.FromMinutes(5),
                PeriodSize.FifteenMinutes => TimeSpan.FromMinutes(15),
                PeriodSize.ThirtyMinutes => TimeSpan.FromMinutes(30),
                PeriodSize.OneHour => TimeSpan.FromHours(1),
                PeriodSize.TwoHours => TimeSpan.FromHours(2),
                PeriodSize.FourHours => TimeSpan.FromHours(4),
                PeriodSize.Day => TimeSpan.FromDays(1),
                PeriodSize.Week => TimeSpan.FromDays(7),
                _ => TimeSpan.Zero
            };
        }

        internal static PeriodSize? ToPeriodSize(this TimeSpan timeSpan)
        {
            PeriodSize? _periodSize = null;

            if (timeSpan == TimeSpan.FromMinutes(1))
                _periodSize = PeriodSize.OneMinute;
            if (timeSpan == TimeSpan.FromMinutes(2))
                _periodSize = PeriodSize.TwoMinutes;
            if (timeSpan == TimeSpan.FromMinutes(3))
                _periodSize = PeriodSize.ThreeMinutes;
            if (timeSpan == TimeSpan.FromMinutes(5))
                _periodSize = PeriodSize.FiveMinutes;
            if (timeSpan == TimeSpan.FromMinutes(15))
                _periodSize = PeriodSize.FifteenMinutes;
            if (timeSpan == TimeSpan.FromMinutes(30))
                _periodSize = PeriodSize.ThirtyMinutes;
            if (timeSpan == TimeSpan.FromHours(1))
                _periodSize = PeriodSize.OneHour;
            if (timeSpan == TimeSpan.FromHours(2))
                _periodSize = PeriodSize.TwoHours;
            if (timeSpan == TimeSpan.FromHours(4))
                _periodSize = PeriodSize.FourHours;
            if (timeSpan == TimeSpan.FromDays(1))
                _periodSize = PeriodSize.Day;
            if (timeSpan == TimeSpan.FromDays(7))
                _periodSize = PeriodSize.Week;

            return _periodSize;
        }

        // DETERMINE DECIMAL PLACES
        internal static int GetDecimalPlaces(this decimal n)
        {
            // source: https://stackoverflow.com/a/30205131/4496145

            n = Math.Abs(n); //make sure it is positive.
            n -= (int)n;     //remove the integer part of the number.
            int decimalPlaces = 0;
            while (n > 0)
            {
                decimalPlaces++;
                n *= 10;
                n -= (int)n;
            }
            return decimalPlaces;
        }
    }
}
