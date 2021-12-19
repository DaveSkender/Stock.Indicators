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

            // convert quotes
            List<QuoteD> quotesList = quotes.ConvertToList();

            // check parameter arguments
            ValidateVwap(quotesList, startDate);

            // initialize
            int size = quotesList.Count;
            startDate = (startDate == null) ? quotesList[0].Date : startDate;
            List<VwapResult> results = new(size);

            double? cumVolume = 0;
            double? cumVolumeTP = 0;

            // roll through quotes
            for (int i = 0; i < size; i++)
            {
                QuoteD q = quotesList[i];
                double? v = q.Volume;
                double? h = q.High;
                double? l = q.Low;
                double? c = q.Close;

                VwapResult r = new()
                {
                    Date = q.Date
                };

                if (q.Date >= startDate)
                {
                    cumVolume += v;
                    cumVolumeTP += v * (h + l + c) / 3;

                    r.Vwap = (cumVolume != 0) ? (decimal?)(cumVolumeTP / cumVolume) : null;
                }

                results.Add(r);
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<VwapResult> RemoveWarmupPeriods(
            this IEnumerable<VwapResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.Vwap != null);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateVwap(
            List<QuoteD> quotesList,
            DateTime? startDate)
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
