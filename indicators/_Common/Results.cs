using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    // RESULT MODELS

    public interface IResult
    {
        public DateTime Date { get; }
    }

    [Serializable]
    public class ResultBase : IResult
    {
        public DateTime Date { get; set; }
    }


    // HELPER FUNCTIONS

    public static partial class Indicator
    {

        // FIND by DATE
        public static TResult Find<TResult>(
            this IEnumerable<TResult> results,
            DateTime lookupDate)
            where TResult : IResult
        {
            return results.FirstOrDefault(x => x.Date == lookupDate);
        }


        // PRUNE SPECIFIC PERIODS extension
        public static IEnumerable<TResult> PruneWarmupPeriods<TResult>(
            this IEnumerable<TResult> results,
            int prunePeriods)
            where TResult : IResult
        {
            return prunePeriods < 0
                ? throw new ArgumentOutOfRangeException(nameof(prunePeriods), prunePeriods,
                    "If specified, the Prune Periods value must be greater than or equal to 0.")
                : results.Prune(prunePeriods);
        }


        // PRUNE RESULTS
        internal static IEnumerable<TResult> Prune<TResult>(
            this IEnumerable<TResult> results,
            int prunePeriods)
            where TResult : IResult
        {
            List<TResult> resultsList = results.ToList();

            if (resultsList.Count <= prunePeriods)
            {
                return new List<TResult>();
            }
            else
            {
                if (prunePeriods > 0)
                {
                    for (int i = 0; i < prunePeriods; i++)
                    {
                        resultsList.RemoveAt(0);
                    }
                }

                return resultsList;
            }
        }
    }
}
