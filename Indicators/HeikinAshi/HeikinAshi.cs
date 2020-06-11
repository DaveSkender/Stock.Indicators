using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // HEIKIN-ASHI
        public static IEnumerable<HeikinAshiResult> GetHeikinAshi(IEnumerable<Quote> history)
        {

            // clean quotes
            history = Cleaners.PrepareHistory(history);

            // check exceptions
            int qtyHistory = history.Count();
            int minHistory = 2;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for Heikin-Ashi.  " +
                        string.Format("You provided {0} periods of history when {1} is required.", qtyHistory, minHistory));
            }

            // initialize
            List<HeikinAshiResult> results = new List<HeikinAshiResult>();

            decimal? prevOpen = null;
            decimal? prevClose = null;
            bool? prevTrend = null;

            foreach (Quote h in history)
            {

                // close
                decimal close = (h.Open + h.High + h.Low + h.Close) / 4;

                // open
                decimal open = (prevOpen == null) ? (h.Open + h.Close) / 2 : (decimal)(prevOpen + prevClose) / 2;

                // high
                decimal[] arrH = { h.High, open, close };
                decimal high = arrH.Max();

                // low
                decimal[] arrL = { h.Low, open, close };
                decimal low = arrL.Min();

                // trend (bullish (buy / green), bearish (sell / red)
                // strength (size of directional shadow / no shadow is strong)
                bool? trend;
                decimal strength;

                if (close > open)
                {
                    trend = true;
                    strength = open - low;
                }
                else if (close < open)
                {
                    trend = false;
                    strength = high - open;
                }
                else
                {
                    trend = prevTrend;
                    strength = high - low;
                }

                HeikinAshiResult result = new HeikinAshiResult
                {
                    Index = (int)h.Index,
                    Date = h.Date,
                    Open = open,
                    High = high,
                    Low = low,
                    Close = close,
                    IsBullish = trend,
                    Weakness = strength,
                };
                results.Add(result);

                // save for next iteration
                prevOpen = open;
                prevClose = close;
                prevTrend = trend;
            }

            return results;
        }

    }

}
