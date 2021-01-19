using System;

namespace Skender.Stock.Indicators
{
    internal static class Functions
    {

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
    }
}
