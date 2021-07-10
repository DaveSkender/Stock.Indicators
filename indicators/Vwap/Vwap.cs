using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // VOLUME WEIGHTED AVERAGE PRICE
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<VwapResult> GetVwap<TQuote>(
            this IEnumerable<TQuote> quotes,
            DateTime? startDate = null)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateVwap(quotesList, startDate);

            // initialize
            int size = quotesList.Count;
            startDate = (startDate == null) ? quotesList[0].Date : startDate;
            List<VwapResult> results = new(size);

            decimal cumVolume = 0m;
            decimal cumVolumeTP = 0m;

            // roll through quotes
            for (int i = 0; i < size; i++)
            {
                TQuote q = quotesList[i];

                VwapResult r = new()
                {
                    Date = q.Date
                };

                if (q.Date >= startDate)
                {
                    cumVolume += q.Volume;
                    cumVolumeTP += q.Volume * (q.High + q.Low + q.Close) / 3;

                    r.Vwap = (cumVolume != 0) ? cumVolumeTP / cumVolume : null;
                }

                results.Add(r);
            }

            return results;
        }


        // remove recommended periods extensions
        public static IEnumerable<VwapResult> RemoveWarmupPeriods(
            this IEnumerable<VwapResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.Vwap != null);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateVwap<TQuote>(
            List<TQuote> quotesList,
            DateTime? startDate)
            where TQuote : IQuote
        {

            // check quotes: done under Sort() for 0 length

            // check parameter arguments (intentionally after quotes check)
            if (startDate < quotesList[0].Date)
            {
                throw new ArgumentOutOfRangeException(nameof(startDate), startDate,
                    "Start Date must be within the quotes range for VWAP.");
            }
        }
    }
}
