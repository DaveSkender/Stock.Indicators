namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // RELATIVE STRENGTH INDEX
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<RsiResult> GetRsi<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods = 14)
            where TQuote : IQuote
        {

            // convert quotes
            List<BasicD> bdList = quotes.ConvertToBasic(CandlePart.Close);

            // calculate
            return CalcRsi(bdList, lookbackPeriods);
        }

        // convert to quotes
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Convert"]/*' />
        ///
        public static IEnumerable<Quote> ConvertToQuotes(
            this IEnumerable<RsiResult> results)
        {
            return results
              .Where(x => x.Rsi != null)
              .Select(x => new Quote
              {
                  Date = x.Date,
                  Open = (decimal)x.Rsi,
                  High = (decimal)x.Rsi,
                  Low = (decimal)x.Rsi,
                  Close = (decimal)x.Rsi,
                  Volume = (decimal)x.Rsi
              })
              .ToList();
        }

        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<RsiResult> RemoveWarmupPeriods(
            this IEnumerable<RsiResult> results)
        {
            int n = results
                .ToList()
                .FindIndex(x => x.Rsi != null);

            return results.Remove(10 * n);
        }


        // internals
        private static List<RsiResult> CalcRsi(List<BasicD> bdList, int lookbackPeriods)
        {

            // check parameter arguments
            ValidateRsi(bdList, lookbackPeriods);

            // initialize
            double lastValue = bdList[0].Value;
            double avgGain = 0;
            double avgLoss = 0;

            int size = bdList.Count;
            List<RsiResult> results = new(size);
            double[] gain = new double[size]; // gain
            double[] loss = new double[size]; // loss

            // roll through quotes
            for (int i = 0; i < bdList.Count; i++)
            {
                BasicD h = bdList[i];
                int index = i + 1;

                RsiResult r = new()
                {
                    Date = h.Date
                };
                results.Add(r);

                gain[i] = (h.Value > lastValue) ? h.Value - lastValue : 0;
                loss[i] = (h.Value < lastValue) ? lastValue - h.Value : 0;
                lastValue = h.Value;

                // calculate RSI
                if (index > lookbackPeriods + 1)
                {
                    avgGain = (avgGain * (lookbackPeriods - 1) + gain[i]) / lookbackPeriods;
                    avgLoss = (avgLoss * (lookbackPeriods - 1) + loss[i]) / lookbackPeriods;

                    if (avgLoss > 0)
                    {
                        double rs = avgGain / avgLoss;
                        r.Rsi = 100 - (100 / (1 + rs));
                    }
                    else
                    {
                        r.Rsi = 100;
                    }
                }

                // initialize average gain
                else if (index == lookbackPeriods + 1)
                {
                    double sumGain = 0;
                    double sumLoss = 0;

                    for (int p = 1; p <= lookbackPeriods; p++)
                    {
                        sumGain += gain[p];
                        sumLoss += loss[p];
                    }
                    avgGain = sumGain / lookbackPeriods;
                    avgLoss = sumLoss / lookbackPeriods;

                    r.Rsi = (avgLoss > 0) ? 100 - (100 / (1 + (avgGain / avgLoss))) : 100;
                }
            }

            return results;
        }


        // parameter validation
        private static void ValidateRsi(
            List<BasicD> quotes,
            int lookbackPeriods)
        {

            // check parameter arguments
            if (lookbackPeriods < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for RSI.");
            }

            // check quotes
            int qtyHistory = quotes.Count;
            int minHistory = lookbackPeriods + 100;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for RSI.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.  "
                    + "Since this uses a smoothing technique, "
                    + "we recommend you use at least {2} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, Math.Max(10 * lookbackPeriods, minHistory));

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
