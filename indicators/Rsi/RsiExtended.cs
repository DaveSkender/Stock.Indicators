using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // RELATIVE STRENGTH INDEX (EXTENDED VERSION with DIVERGENCE)
        /// <include file='./info.xml' path='indicators/type[@name="Extended"]/*' />
        /// 
        public static IEnumerable<RsiExtendedResult> GetRsiExtended<TQuote>(
            this IEnumerable<TQuote> quotes,
            int rsiPeriods = 14,
            int maxTrendPeriods = 60,
            int fractalPeriods = 2)
            where TQuote : IQuote
        {

            // check parameter arguments
            ValidateRsiExtended(rsiPeriods, maxTrendPeriods, fractalPeriods);

            // convert history to basic format
            List<BasicData> bdList = quotes.ConvertToBasic("C");
            int size = bdList.Count;

            // calculate RSI
            List<RsiExtendedResult> results = CalcRsi(bdList, rsiPeriods)
                .Select(x => new RsiExtendedResult
                {
                    Date = x.Date,
                    Rsi = x.Rsi
                })
                .ToList();

            // convert RSI to synthetic quotes
            IEnumerable<Quote> rsiQuotes = results
                .Where(x => x.Rsi != null)
                .Select(x => new Quote
                {
                    Date = x.Date,
                    Open = (decimal)x.Rsi,
                    Close = (decimal)x.Rsi
                });

            // get fractal information
            List<FractalResult> pfList = GetFractal(quotes, fractalPeriods, EndType.Close).ToList();
            List<FractalResult> rsList = GetFractal(rsiQuotes, fractalPeriods, EndType.Close).ToList();

            // restore missing RSI fractal records (due to exclusion of RSI nulls)
            for (int i = 0; i < size; i++)
            {
                BasicData bd = bdList[i];
                FractalResult rd = rsList.FirstOrDefault(x => x.Date == bd.Date);

                if (rd == null)
                {
                    rsList.Add(new FractalResult { Date = bd.Date });
                }
            }

            List<FractalResult> rfList = rsList
                .OrderBy(x => x.Date).ToList();

            // roll through quotes
            for (int i = maxTrendPeriods - 1; i < size; i++)
            {
                RsiExtendedResult r = results[i];

                // TODO: figure this out
                // https://www.babypips.com/learn/forex/divergence-cheat-sheet

                // TODO: may be easier to work back from identified
                // fractal at (i) when it is an H or an L to see if there are
                // prior H or Ls to satisfy the four categories

                // find HH/LL price trends in lookback window
                for (int p = i - maxTrendPeriods; p < i; p++)
                {
                    // only do this if fractal identified at (i)

                    // if PR fractal

                    // if OS fractal

                    // only if PR and OS "zones" overlap?
                }
            }

            return results;
        }

        // validation
        private static void ValidateRsiExtended(
            int rsiPeriods,
            int maxTrendPeriods,
            int fractalPeriods)
        {

            // RSI period is validated in CalcRSI
            // Fractal period is validated in GetFractal

            // check parameter arguments
            if (maxTrendPeriods < rsiPeriods || maxTrendPeriods < fractalPeriods)
            {
                throw new ArgumentOutOfRangeException(nameof(maxTrendPeriods), maxTrendPeriods,
                    "Max trend period must be at least 2 for RSI Divergence.");
            }
        }
    }
}
