using System;
using System.Linq;

namespace Skender.Stock.Indicators
{
    internal static class Functions
    {

        internal static double StdDev(double[] values)
        {
            // ref: https://stackoverflow.com/questions/2253874/standard-deviation-in-linq

            double sd = 0;
            int count = values.Length;
            if (count > 1)
            {
                double avg = values.Average();
                double sum = values.Sum(d => (d - avg) * (d - avg));
                sd = Math.Sqrt(sum / count);
            }
            return sd;
        }

    }
}
