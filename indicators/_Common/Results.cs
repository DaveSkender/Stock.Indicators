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
            ValidatePrunePeriods(prunePeriods);
            return results.Prune(prunePeriods);
        }

        // VALIDATE PRUNE PERIODS
        internal static void ValidatePrunePeriods(int prunePeriods)
        {
            if (prunePeriods < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(prunePeriods), prunePeriods,
                    "If specified, the Prune Periods value must be greater than or equal to 0.");
            }
        }

        // PRUNE RESULTS
        internal static IEnumerable<TResult> Prune<TResult>(
            this IEnumerable<TResult> ogResults,
            int prunePeriods)
            where TResult : IResult
        {
            List<TResult> ogResultsList = ogResults.ToList();
            int ogSize = ogResultsList.Count;

            List<TResult> prunedResults = new();

            if (ogSize <= prunePeriods)
            {
                return prunedResults;
            }
            else
            {
                // note: TakeLast() is not .NET Framework compatible
                for (int i = prunePeriods; i < ogSize; i++)
                {
                    prunedResults.Add(ogResultsList[i]);
                }

                return prunedResults;
            }
        }
    }
}
