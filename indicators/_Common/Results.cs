using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    // RESULT MODELS

    public interface IResultBase
    {
        public DateTime Date { get; }
    }

    [Serializable]
    public class ResultBase : IResultBase
    {
        public DateTime Date { get; set; }
    }


    // HELPER FUNCTIONS

    public static class IndicatorResults
    {

        public static TResult Find<TResult>(
            this IEnumerable<TResult> results,
            DateTime lookupDate)
            where TResult : IResultBase
        {
            return results.FirstOrDefault(x => x.Date == lookupDate);
        }
    }
}
