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
    }
}
