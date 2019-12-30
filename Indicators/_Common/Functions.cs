using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    internal static class Functions
    {

        internal static double StdDev(IEnumerable<double> values)
        {
            // ref: https://stackoverflow.com/questions/2253874/standard-deviation-in-linq

            double ret = 0;
            int count = values.Count();
            if (count > 1)
            {
                //Compute the Average
                double avg = values.Average();

                //Perform the Sum of (value-avg)^2
                double sum = values.Sum(d => (d - avg) * (d - avg));

                //Put it all together
                ret = Math.Sqrt(sum / count);
            }
            return ret;
        }

    }
}
