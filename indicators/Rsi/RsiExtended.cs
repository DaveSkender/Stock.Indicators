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
            IEnumerable<TQuote> history,
            int rsiPeriod = 14,
            int maxTrendPeriod = 60,
            int fractalPeriod = 2)
            where TQuote : IQuote
        {

            // check parameter arguments
            ValidateRsiExtended(rsiPeriod, maxTrendPeriod, fractalPeriod);

            // convert history to basic format
            List<BasicData> bdList = history.ConvertToBasic("C");
            int size = bdList.Count;

            // calculate RSI
            List<RsiExtendedResult> results = CalcRsi(bdList, rsiPeriod)
                .Select(x => new RsiExtendedResult
                {
                    Date = x.Date,
                    Rsi = x.Rsi
                })
                .ToList();

            // convert RSI to synthetic history
            IEnumerable<Quote> rsiHistory = results
                .Where(x => x.Rsi != null)
                .Select(x => new Quote
                {
                    Date = x.Date,
                    Open = (decimal)x.Rsi,
                    Close = (decimal)x.Rsi
                });

            // get fractal information
            List<FractalResult> pfList = GetFractal(history, fractalPeriod, EndType.Close).ToList();
            List<FractalResult> rsList = GetFractal(rsiHistory, fractalPeriod, EndType.Close).ToList();

            // restore missing RSI fractal records (due to exclusion of RSI nulls)
            for (int i = 0; i < size; i++)
            {
                BasicData bd = bdList[i];
                FractalResult rd = rsList.Where(x => x.Date == bd.Date).FirstOrDefault();

                if (rd == null)
                {
                    rsList.Add(new FractalResult { Date = bd.Date });
                }
            }

            List<FractalResult> rfList = rsList
                .OrderBy(x => x.Date).ToList();

            // roll through history
            for (int i = maxTrendPeriod - 1; i < size; i++)
            {
                RsiExtendedResult r = results[i];

                // TODO: figure this out
                // https://www.babypips.com/learn/forex/divergence-cheat-sheet

                // TODO: may be easier to work back from identified
                // fractal at (i) when it is an H or an L to see if there are
                // prior H or Ls to satisfy the four categories

                // find HH/LL price trends in lookback window
                for (int p = i - maxTrendPeriod; p < i; p++)
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
            int rsiPeriod,
            int maxTrendPeriod,
            int fractalPeriod)
        {

            // RSI period is validated in CalcRSI
            // Fractal period is validated in GetFractal

            // check parameter arguments
            if (maxTrendPeriod < rsiPeriod || maxTrendPeriod < fractalPeriod)
            {
                throw new ArgumentOutOfRangeException(nameof(maxTrendPeriod), maxTrendPeriod,
                    "Max trend period must be at least 2 for RSI Divergence.");
            }
        }
    }
}
