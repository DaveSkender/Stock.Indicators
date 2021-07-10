﻿using System;
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
            List<TQuote> historyList = quotes.Sort();

            // check parameter arguments
            ValidateVwap(historyList, startDate);

            // initialize
            int size = historyList.Count;
            startDate = (startDate == null) ? historyList[0].Date : startDate;
            List<VwapResult> results = new(size);

            decimal cumVolume = 0m;
            decimal cumVolumeTP = 0m;

            // roll through quotes
            for (int i = 0; i < size; i++)
            {
                TQuote h = historyList[i];

                VwapResult r = new()
                {
                    Date = h.Date
                };

                if (h.Date >= startDate)
                {
                    cumVolume += h.Volume;
                    cumVolumeTP += h.Volume * (h.High + h.Low + h.Close) / 3;

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
            List<TQuote> historyList,
            DateTime? startDate)
            where TQuote : IQuote
        {

            // check quotes: done under Sort() for 0 length

            // check parameter arguments (intentionally after quotes check)
            if (startDate < historyList[0].Date)
            {
                throw new ArgumentOutOfRangeException(nameof(startDate), startDate,
                    "Start Date must be within the quotes range for VWAP.");
            }
        }
    }
}
