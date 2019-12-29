using System.Collections.Generic;
using System.Linq;

namespace StockIndicators
{
    public static partial class Indicators
    {
        // HEIKIN-ASHI
        public static IEnumerable<HeikinAshiResult> GetHeikinAshi(IEnumerable<Quote> history)
        {

            // clean quotes
            history = Cleaners.PrepareHistory(history);

            List<HeikinAshiResult> results = new List<HeikinAshiResult>();

            decimal? prevOpen = null;
            decimal? prevClose = null;

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
                bool trend;
                decimal strength;

                if (close > open)
                {
                    trend = true;
                    strength = open - low;
                }
                else
                {
                    trend = false;
                    strength = high - open;
                }

                HeikinAshiResult result = new HeikinAshiResult
                {
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
            }

            return results;
        }

    }

}
